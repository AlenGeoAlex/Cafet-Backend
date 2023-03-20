using AutoMapper;
using Cafet_Backend.Context;
using Cafet_Backend.Dto;
using Cafet_Backend.Interfaces;
using Cafet_Backend.Models;
using Cafet_Backend.Provider;
using Cafet_Backend.Specification;
using Microsoft.EntityFrameworkCore;

namespace Cafet_Backend.Repository;

public class StatisticsRepository : IStatisticsRepository
{
    public readonly CafeContext Context;
    public readonly ILogger<StatisticsRepository> Logger;
    public readonly IMapper Mapper;

    public StatisticsRepository(CafeContext context, ILogger<StatisticsRepository> logger, IMapper mapper)
    {
        Context = context;
        Logger = logger;
        Mapper = mapper;
    }

    public async Task<List<UserActivity>> GetUserActivityAsync(
        UserOrderActivitySpecification? orderActivitySpecification,
        UserWalletActivitySpecification? walletActivitySpecification)
    {
        List<UserActivity> userActivities = new List<UserActivity>();

        if (orderActivitySpecification != null)
        {
            List<Order> listAsync = await SpecificationProvider<Order, Guid>
                .GetQuery(Context.Set<Order>(), orderActivitySpecification)
                .Where(o => o.OrderDelivered != null)
                .Include(o => o.OrderItems)
                .ToListAsync();

            foreach (Order order in listAsync)
            {
                UserActivity newActivity = new UserActivity()
                {
                    ActivityType = order.Cancelled ? ActivityType.CancelledOrder.ToString() : ActivityType.CompletedOrder.ToString(),
                    
                    ActivityId = order.Id.ToString(),
                    
                    Amount = order.OrderAmount,
                    
                    ActivityOccurence = order.OrderPlaced.ToShortDateString() 
                                        + " " 
                                        + order.OrderDelivered?.ToShortTimeString(),
                    
                    ActivityOccurenceRaw = order.OrderPlaced.Ticks,
                    
                    ActivityCompleted = order.OrderDelivered?.ToShortDateString() 
                                        + " " 
                                        + order.OrderDelivered?.ToShortTimeString(),
                    
                    ActivityCompletedRaw = order.OrderDelivered?.Ticks,
                    
                    Others = GetConcatedNameOf(order.OrderItems)
                };
                
                userActivities.Add(newActivity);
            }
        }
        
        if (walletActivitySpecification != null)
        {
            List<WalletHistory> listAsync = await SpecificationProvider<WalletHistory, int>
                .GetQuery(Context.Set<WalletHistory>(), walletActivitySpecification)
                .Where(o => o.RechargeStatus)
                .ToListAsync();

            foreach (WalletHistory walletActivity in listAsync)
            {
                UserActivity newActivity = new UserActivity()
                {
                    ActivityId = walletActivity.Id.ToString(),
                    ActivityType = walletActivity.Credit ? ActivityType.WalletCredit.ToString() : ActivityType.WalletDebit.ToString(),
                    ActivityOccurence = walletActivity.RechargeTime.ToShortDateString() 
                                        + " " 
                                        + walletActivity.RechargeTime.ToShortTimeString(),
                    
                    ActivityOccurenceRaw = walletActivity.RechargeTime.Ticks,
                    
                    ActivityCompleted = walletActivity.RechargeTime.ToShortDateString() 
                    + " " 
                    + walletActivity.RechargeTime.ToShortTimeString(),
                    
                    ActivityCompletedRaw = walletActivity.RechargeTime.Ticks,
                    
                    Amount = walletActivity.Amount
                };
                
                userActivities.Add(newActivity);
            }
        }

        if (userActivities.Count <= 0)
            return userActivities;

        userActivities.Sort((a, b) => a.ActivityOccurenceRaw.CompareTo(b.ActivityOccurenceRaw));

        return userActivities;
    }

    public async Task<List<RevenueReportDto>> GetRevenueAsync(int year)
    {
        var listAsync = await Context.Orders
            .Where(x => x.OrderDelivered != null && x.OrderPlaced.Year == year)
            .GroupBy(x => x.OrderPlaced.Month)
            .Select(x => new
            {
                Id = x.Key,
                Sum = x.Sum(s => s.OrderAmount)
            })
            .ToListAsync();

        List<RevenueReportDto> reportDtos = new List<RevenueReportDto>();
        foreach (var x1 in listAsync)
        {
           reportDtos.Add(new RevenueReportDto(){Month = x1.Id, Revenue = x1.Sum});
        }
        return reportDtos;
    }

    /// <summary>
    /// Get the top sold items of the day
    /// </summary>
    /// <param name="count"></param>
    /// <returns>
    /// Returns a dictionary with food id as key and total ordered quantity as value 
    /// </returns>
    public async Task<List<TopSelledFood>> GetTopSoldOfDay(int count)
    {
        if (count <= 0)
            count = 3;

        return await Context.OrderItems
            .Include(x => x.Order)
            .Where(x => x.Order.OrderPlaced.Date == DateTime.Today)
            .GroupBy(x => x.FoodId)
            .Select(
                t => new TopSelledFood()
                {
                    FoodId = t.Key,
                    Quantity = t.Sum(x => x.Quantity),
                })
            .OrderByDescending(x => x.Quantity)
            .Take(count)
            .ToListAsync();
    }

    public async Task<int> GetTotalOrderCount()
    {
        return await Context.Orders.CountAsync();
    }

    public async Task<int> GetUserCount()
    {
        return await Context.Users.CountAsync();
    }

    public async Task<int> GetOrderCountBy(Specification<Order> specification)
    {
        return await SpecificationProvider<Order, Guid>.GetQuery(Context.Set<Order>(), specification)
            .CountAsync();
    }

    public async Task<double> GetCostReturn(Specification<Order> specification)
    {
        return await SpecificationProvider<Order, Guid>.GetQuery(Context.Set<Order>(), specification).SumAsync(x => x.OrderAmount);
    }
    
    public async Task<StatisticsDto> GetCurrentStatistics()
    {
        Specification<Order> specification = new Specification<Order>();
        specification.AddFilterCondition(x => x.OrderDelivered != null);

        int userCount = await GetUserCount();
        int orderCountBy = await GetOrderCountBy(specification);
        double costReturn = await GetCostReturn(specification);
        int totalOrderCount = await GetTotalOrderCount();

        StatisticsDto statisticsDto = new StatisticsDto()
        {
            CompletedOrders = orderCountBy,
            CostEarned = costReturn,
            TotalOrders = totalOrderCount,
            UserCount = userCount
        };

        return statisticsDto;
    }

    private string? GetConcatedNameOf(List<OrderItems> orderItemsList)
    {
        string s = "";
        foreach (OrderItems orderItems in orderItemsList)
        {
            s += " " + orderItems.FoodName;
            if(s.Length <= 20)
                break;
        }
        return s;
    }
}