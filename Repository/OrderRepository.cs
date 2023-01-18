using AutoMapper;
using Cafet_Backend.Context;
using Cafet_Backend.Dto;
using Cafet_Backend.Interfaces;
using Cafet_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Cafet_Backend.Repository;

public class OrderRepository : IOrderRepository
{

    private readonly CafeContext CafeContext;
    private readonly ILogger<OrderRepository> Logger;
    private readonly IMapper Mapper;

    public OrderRepository(CafeContext cafeContext, IMapper mapper, ILogger<OrderRepository> logger)
    {
        CafeContext = cafeContext;
        Mapper = mapper;
        Logger = logger;
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
            OrderPlacedForId = orderPlacedFor.Id
        };

        await CafeContext.Orders.AddAsync(order);
        await CafeContext.SaveChangesAsync();

        Console.WriteLine(order.Id);
        
        return order;
    }

    public async void DeleteOrderDataOfAsync(int userId)
    {
        List<Order> ordersPlacedByUser = await CafeContext.Orders.Where(o => o.OrderPlacedById == userId).ToListAsync();
        CafeContext.RemoveRange(ordersPlacedByUser);
        await CafeContext.SaveChangesAsync();
    }
    
}