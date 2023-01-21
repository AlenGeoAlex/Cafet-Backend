using System.Text;
using AutoMapper;
using Cafet_Backend.Context;
using Cafet_Backend.Dto;
using Cafet_Backend.Interfaces;
using Cafet_Backend.Manager;
using Cafet_Backend.Models;
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
    public readonly IMapper Mapper;

    public OrderRepository(CafeContext cafeContext, IMapper mapper, ILogger<OrderRepository> logger, MailModelManager manager, IMailService mailService)
    {
        CafeContext = cafeContext;
        Mapper = mapper;
        Logger = logger;
        MailModelManager = manager;
        MailService = mailService;
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
            OrderPlacedForId = orderPlacedFor.Id
        };

        await CafeContext.Orders.AddAsync(order);
        await CafeContext.SaveChangesAsync();

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
    
}