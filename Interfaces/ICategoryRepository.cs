using Cafet_Backend.Models;

namespace Cafet_Backend.Interfaces;

public interface ICategoryRepository
{
    Task<FoodCategory?> GetByIdAsync(int id);

    Task<FoodCategory?> GetByNameAsync(string name);

    Task<List<FoodCategory?>> GetAllCategoryAsync();

    Task<bool> ContainsAsync(string name);

    Task<int> RegisterAsync(FoodCategory category);
    
    Task<int> UpdateAsync(FoodCategory category);

    Task<int> DeleteAsync(int id);
    
}