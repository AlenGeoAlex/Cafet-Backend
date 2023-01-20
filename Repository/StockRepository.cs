using System.Linq.Expressions;
using AutoMapper;
using Cafet_Backend.Context;
using Cafet_Backend.Dto;
using Cafet_Backend.Dto.InputDtos;
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

    public async Task<bool> RegisterDailyStockAsync(List<DailyStock> stocks, bool clear)
    {
        if(clear)
            await ClearCurrentStockAsync();
        await CafeContext.Stocks.AddRangeAsync(stocks);
        await CafeContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteStockAsync(int id)
    {
        DailyStock? firstOrDefaultAsync = await CafeContext.Stocks
            .FirstOrDefaultAsync(stock => stock.Id == id);

        
        if (firstOrDefaultAsync == null)
            return false;

        CafeContext.Stocks.Remove(firstOrDefaultAsync);
        await CafeContext.SaveChangesAsync();
        return true;
    }
    
    public async Task<bool> DeleteStockAsync(List<int> id)
    {
        List<DailyStock?> dailyStocks = await CafeContext.Stocks.Where(st => id.Contains(st.Id)).ToListAsync();

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
    
    public async Task<List<DailyStock>> GetAllStockAsync(string? ignoreStock, string? foodCategory)
    {
        bool ignoreOutOfStock = !string.IsNullOrEmpty(ignoreStock) && ignoreStock == "true";

        return await CafeContext.Stocks
            .Include(stock => stock.Food)
            .Include(stock => stock.Food.Category )
            .ToListAsync();
    }

    public async Task<List<DailyStock>> GetStockFromFoodName(string queryString)
    {
        return await CafeContext.Stocks
            .Include(stock => stock.Food)
            .Include(stock => stock.Food.Category)
            .Where(stock => stock.Food.Name.Contains(queryString) || stock.Food.Tags.Contains(queryString))
            .ToListAsync();
    }

    public async Task<DailyStock?> GetStockOfId(int id)
    {
        return await CafeContext.Stocks
            .Include(stock => stock.Food)
            .Include(stock => stock.Food.Category)
            .FirstOrDefaultAsync(stock => stock.Id == id);
    }
    
    public async Task<DailyStock?> GetStockOfFoodId(int id)
    {
        return await CafeContext.Stocks
            .Include(stock => stock.Food)
            .Include(stock => stock.Food.Category)
            .FirstOrDefaultAsync(stock => stock.FoodId == id);
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

    public async Task SaveChangesAsync()
    {
        await CafeContext.SaveChangesAsync();
        return;
    }

    public async Task<List<DailyStock>> GetStockOfFoodIds(List<int> FoodIds)
    {
        return await CafeContext.Stocks
            .Include(stock => stock.Food)
            .Where(stock => FoodIds.Contains(stock.FoodId))
            .ToListAsync();
    }

    public async Task<ProcessedOrder> ProcessOrderResponse(List<FoodOrder> foodOrders)
    {
        double cost = 0D;
        ProcessedOrder response = new ProcessedOrder(foodOrders);
        List<int> orderedFoodIds = response.GetAllFoodIds();
        List<DailyStock> stockOfFoodIds = await GetStockOfFoodIds(orderedFoodIds);
        
        foreach (DailyStock dailyStock in stockOfFoodIds)
        {
            int eachFoodId = dailyStock.FoodId;
            long inQuantity = dailyStock.CurrentStock;

            FoodOrder? @default = foodOrders.FirstOrDefault(order => order.FoodId == eachFoodId);
            if(@default == null)
                continue;

            int requiredQuantity = @default.OrderQuantity;

            if (requiredQuantity <= inQuantity)
            {
                response.SetAvailableOfFoodId(eachFoodId);
                cost += (dailyStock.Food.FoodPrice * requiredQuantity);
            }
        }
        
        if (response.Validate())
            response.OrderCost = cost;

        return response;
    }
}