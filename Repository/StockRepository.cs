using AutoMapper;
using Cafet_Backend.Context;
using Cafet_Backend.Dto;
using Cafet_Backend.Interfaces;
using Cafet_Backend.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Cafet_Backend;

public class StockRepository : IStockRepository
{

    private readonly CafeContext CafeContext;
    private readonly IMapper Mapper;

    public StockRepository(CafeContext cafeContext, IMapper mapper)
    {
        CafeContext = cafeContext;
        Mapper = mapper;
    }

    public async Task<bool> ClearCurrentStockAsync()
    {
        await CafeContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE [Stocks];");
        return true;
    }

    public async Task<bool> RegisterDailyStockAsync(List<DailyStock> stocks)
    {
        await ClearCurrentStockAsync();
        await CafeContext.Stocks.AddRangeAsync(stocks);
        await CafeContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteStockAsync(int id)
    {
        DailyStock? firstOrDefaultAsync = await CafeContext.Stocks
            .FirstOrDefaultAsync(stock => stock.Id == id);

        Console.WriteLine(JsonConvert.SerializeObject(firstOrDefaultAsync));
        
        if (firstOrDefaultAsync == null)
            return false;

        CafeContext.Stocks.Remove(firstOrDefaultAsync);
        await CafeContext.SaveChangesAsync();
        return true;
    }
    
    public async Task<bool> DeleteStockAsync(List<int> id)
    {
        List<DailyStock> dailyStocks = await CafeContext.Stocks.Where(st => id.Contains(st.Id)).ToListAsync();

        if (dailyStocks.Count <= 0)
            return false;

        CafeContext.Stocks.RemoveRange(dailyStocks);
        await CafeContext.SaveChangesAsync();
        return true;
    }

    public async Task<List<DailyStock>> GetAllStockAsync()
    {
        return await CafeContext.Stocks
            .Include(stock => stock.Food)
            .Include(stock => stock.Food.Category )
            .ToListAsync();
    }

    public DailyStockDto AsDto(DailyStock ds)
    {
        return Mapper.Map<DailyStock, DailyStockDto>(ds);
    }

    public List<DailyStockDto> AsDto(List<DailyStock> dss)
    {
        List<DailyStockDto> returnDto = new List<DailyStockDto>();

        foreach (DailyStock dailyStock in dss)
        {
            returnDto.Add(AsDto(dailyStock));
        }
        
        return returnDto;
    }

    public async Task<bool> UpdateStockForAsync(DailyStock stock)
    {
        return true;
    }

}