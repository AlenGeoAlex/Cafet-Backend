using Cafet_Backend.Dto;
using Cafet_Backend.Models;

namespace Cafet_Backend.Interfaces;

public interface IStockRepository
{
    Task<bool> ClearCurrentStockAsync();

    Task<bool> RegisterDailyStockAsync(List<DailyStock> stocks);

    Task<List<DailyStock>> GetAllStockAsync();

    DailyStockDto AsDto(DailyStock ds);

    List<DailyStockDto> AsDto(List<DailyStock> dss);

    Task<bool> DeleteStockAsync(int id);

    Task<bool> DeleteStockAsync(List<int> id);

    Task<List<DailyStock>> GetStockFromFoodName(string queryString);

    Task<DailyStock?> GetStockOfId(int id);

    Task<DailyStock?> GetStockOfFoodId(int id);

    Task<List<DailyStock>> GetStockOfFoodIds(List<int> FoodIds);
}