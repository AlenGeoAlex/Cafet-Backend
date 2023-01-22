using AutoMapper;
using Cafet_Backend.Context;
using Cafet_Backend.Dto;
using Cafet_Backend.Interfaces;
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

    public async Task<bool> Test()
    {
        /*var listAsync = Context.OrderItems
            .GroupBy(x => x.FoodId)
            .Select(
                t => new
                {
                    foodId = t.Key,
                    name = t.Select(x => x.FoodName),
                    sum = t.Sum(x => x.Quantity)
                })
            .OrderByDescending(x => x.sum)
            .Take(3)
            .ToDictionary(x => x.name.FirstOrDefault("Unknown"), y => y.sum);*/
        
            return true;
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
}