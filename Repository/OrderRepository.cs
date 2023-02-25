using System.Text;
using AutoMapper;
using Cafet_Backend.Context;
using Cafet_Backend.Dto;
using Cafet_Backend.Hub;
using Cafet_Backend.Interfaces;
using Cafet_Backend.Manager;
using Cafet_Backend.Models;
using Cafet_Backend.Provider;
using Cafet_Backend.Specification;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Cafet_Backend.Repository;

public class OrderRepository : IOrderRepository
{

    public static readonly string OrderTable =
        "<tr>" +
        "<td class='row'>{0}</td>" +
        "<td class='row'>{1}</td>" +
        "<td class='row'>{2}</td>" +
        "<td class='row'>{3}</td>" +
        "<td class='row'>{4}</td>" +
        "</tr>";

    public readonly CafeContext CafeContext;
    public readonly ILogger<OrderRepository> Logger;
    public readonly MailModelManager MailModelManager;
    public readonly IMailService MailService;
    public readonly IHubContext<OrderHub, IOrderHubClient> OrderHub;
    public readonly IMapper Mapper;

    public OrderRepository(CafeContext cafeContext, IMapper mapper, ILogger<OrderRepository> logger, MailModelManager manager, IMailService mailService, IHubContext<OrderHub, IOrderHubClient> orderHub)
    {
        CafeContext = cafeContext;
        Mapper = mapper;
        Logger = logger;
        MailModelManager = manager;
        MailService = mailService;
        OrderHub = orderHub;
    }

    public async Task<List<Order>> GetOrdersFor(ISpecification<Order> param)
    {
        return await SpecificationProvider<Order, Guid>
            .GetQuery(CafeContext.Set<Order>().AsQueryable(), param)
            .Include(or => or.OrderPlacedFor)
            .Include(or => or.OrderItems)
            .ThenInclude(or => or.Food)
            .ThenInclude(or => or.Category)
            .ToListAsync();
    }

    public async Task<Order?> CreateOrderForPayment(ProcessedOrder processedOrder, User orderPlacedBy,
        User orderPlacedFor)
    {
        if (!processedOrder.OrderSuccessful)
            return null;

        List<OrderItems> processedOrders = new List<OrderItems>();
        List<int> allPossibleFoodId = processedOrder.GetAllFoodIds();
        List<Food> foodList = await CafeContext.Foods
            .Where(fo => allPossibleFoodId.Contains(fo.Id))
            .ToListAsync();
        
        foreach (KeyValuePair<int,bool> foodIdOrderStatus in processedOrder.OrderStatus)
        {
            
            int foodId = foodIdOrderStatus.Key;
            Food? @default = foodList.FirstOrDefault(f => f.Id ==foodId);
            if (@default == null)
            {
                Logger.LogWarning("Failed to get the food of id "+foodId);
                return null;
            }

            if (!processedOrder.OrderFoodQuantity.ContainsKey(@default.Id))
            {
                Logger.LogWarning("Failed to get the quantity of food of id "+foodId);
                return null;
            }
            
            OrderItems eachOrderItem = new OrderItems()
            {
                FoodId = @default.Id,
                FoodName = @default.Name,
                FoodPrice = @default.FoodPrice,
                Quantity = processedOrder.OrderFoodQuantity[@default.Id]
            };


            processedOrders.Add(eachOrderItem);
        }
        
                
        //Reduce the quantity at last!
        foreach (KeyValuePair<int,int> foodIdQuantityMap in processedOrder.OrderFoodQuantity)
        {
            int foodId = foodIdQuantityMap.Key;
            int quantity = foodIdQuantityMap.Value;
            
            
            DailyStock? dailyStock = await CafeContext.Stocks.FirstOrDefaultAsync(s => s.FoodId == foodId);
            
            if (dailyStock == null)
                return null;

            if (dailyStock.CurrentStock < quantity)
                return null;

            dailyStock.CurrentStock -= quantity;
        }

        Order order = new Order()
        {
            OrderAmount = processedOrder.OrderCost,
            OrderItems = processedOrders,
            OrderDelivered = null,
            OrderPlaced = DateTime.Now,
            OrderPlacedById = orderPlacedBy.Id,
            OrderPlacedForId = orderPlacedFor.Id,
            PaymentStatus = PaymentStatus.Pending,
            WalletPayment = false
        };

        await CafeContext.Orders.AddAsync(order);
        await CafeContext.SaveChangesAsync();
        
        return order;

    }

    public async Task<string?> MarkOrderAsComplete(Guid orderId)
    {
        Order? orderOfId = await GetOrderOfId(orderId);
        if (orderOfId == null)
        {
            return "An unknown order was provided";
        }


        if (orderOfId.PaymentStatus != PaymentStatus.Pending)
        {
            return "The order was already completed!";
        }

        orderOfId.PaymentStatus = PaymentStatus.Success;
        await CafeContext.SaveChangesAsync();
        await OrderHub.Clients.All.SendOrderUpdate(Mapper.Map<StaffCheckOrderDto>(orderOfId));

        Logger.LogInformation($"Created a new order for {orderOfId.OrderPlacedFor.EmailAddress} with id {orderId.ToString()}");
        return null;
    }

    public async Task<Order?> CreateOrder(ProcessedOrder processedOrder, User orderPlacedBy, User orderPlacedFor)
    {
        if (!processedOrder.OrderSuccessful)
            return null;

        List<OrderItems> processedOrders = new List<OrderItems>();
        List<int> allPossibleFoodId = processedOrder.GetAllFoodIds();

        List<Food> foodList = await CafeContext.Foods
            .Where(fo => allPossibleFoodId.Contains(fo.Id))
            .ToListAsync();

        StringBuilder mailerBuilder = new StringBuilder();
        int counter = 1;
        foreach (KeyValuePair<int,bool> foodIdOrderStatus in processedOrder.OrderStatus)
        {
            
            int foodId = foodIdOrderStatus.Key;
            Food? @default = foodList.FirstOrDefault(f => f.Id ==foodId);
            if (@default == null)
            {
                Logger.LogWarning("Failed to get the food of id "+foodId);
                return null;
            }

            if (!processedOrder.OrderFoodQuantity.ContainsKey(@default.Id))
            {
                Logger.LogWarning("Failed to get the quantity of food of id "+foodId);
                return null;
            }

            string[] orderTablePlaceholder = new[]
            {
                counter.ToString(),
                @default.Name,
                @default.FoodPrice.ToString(),
                processedOrder.OrderFoodQuantity[@default.Id].ToString(),
                (@default.FoodPrice * processedOrder.OrderFoodQuantity[@default.Id]).ToString()
            };
            string orderTable = string.Format(OrderTable, orderTablePlaceholder);
            mailerBuilder.Append(orderTable);
            OrderItems eachOrderItem = new OrderItems()
            {
                FoodId = @default.Id,
                FoodName = @default.Name,
                FoodPrice = @default.FoodPrice,
                Quantity = processedOrder.OrderFoodQuantity[@default.Id]
            };


            processedOrders.Add(eachOrderItem);
            counter += 1;
        }
        
        //Reduce the quantity at last!
        foreach (KeyValuePair<int,int> foodIdQuantityMap in processedOrder.OrderFoodQuantity)
        {
            int foodId = foodIdQuantityMap.Key;
            int quantity = foodIdQuantityMap.Value;
            
            
            DailyStock? dailyStock = await CafeContext.Stocks.FirstOrDefaultAsync(s => s.FoodId == foodId);
            
            if (dailyStock == null)
                return null;

            if (dailyStock.CurrentStock < quantity)
                return null;

            dailyStock.CurrentStock -= quantity;
        }

        Order order = new Order()
        {
            OrderAmount = processedOrder.OrderCost,
            OrderItems = processedOrders,
            OrderDelivered = null,
            OrderPlaced = DateTime.Now,
            OrderPlacedById = orderPlacedBy.Id,
            OrderPlacedForId = orderPlacedFor.Id,
            PaymentStatus = PaymentStatus.Success,
            WalletPayment = true
        };

        await CafeContext.Orders.AddAsync(order);
        await CafeContext.SaveChangesAsync();
        await OrderHub.Clients.All.SendOrderUpdate(Mapper.Map<StaffCheckOrderDto>(order));

        Logger.LogInformation($"Created a new order for {orderPlacedFor.EmailAddress} with id {order.Id}");
        string[] mailBodyPlaceholder = new string[]
        {
            order.Id.ToString(),
            orderPlacedFor.EmailAddress,
            orderPlacedFor.FirstName,
            orderPlacedFor.LastName,
            orderPlacedFor.PhoneNumber ?? "Unknown",
            mailerBuilder.ToString()
        };

        string[] subjectPlaceholder = new string[]
        {
            order.Id.ToString()
        };

        bool mailAsync = await MailService.SendMailAsync(MailModelManager.OrderPlaced, orderPlacedFor.EmailAddress, mailBodyPlaceholder,
            subjectPlaceholder);

        if (!mailAsync)
        {
            Logger.LogWarning("Failed to send order email to "+orderPlacedFor.EmailAddress);
        }


        return order;
    }

    public async void DeleteOrderDataOfAsync(int userId)
    {
        List<Order> ordersPlacedByUser = await CafeContext.Orders.Where(o => o.OrderPlacedById == userId).ToListAsync();
        CafeContext.RemoveRange(ordersPlacedByUser);
        await CafeContext.SaveChangesAsync();
    }

    public async Task<Order?> GetOrderOfId(Guid orderId)
    {
        return await CafeContext.Orders
            .Include(o => o.OrderPlacedFor)
            .Include(o => o.OrderPlacedBy)
            .Include(o => o.OrderItems)
            .ThenInclude(o => o.Food)
            .FirstOrDefaultAsync(o => o.Id == orderId);

    }

    public async Task SaveAsync()
    {
        await CafeContext.SaveChangesAsync();
        return;
    }
}