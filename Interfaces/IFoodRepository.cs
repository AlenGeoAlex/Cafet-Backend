using Cafet_Backend.Models;

namespace Cafet_Backend.Interfaces;

public interface IFoodRepository
{
    Task<Food?> GetFoodByIdAsync(int id);

    Task<Food?> GetFoodByNameAsync(string name);

    Task<IReadOnlyList<Food>> GetMatchingFoodOfNamesAsync(string keyword);

    Task<IReadOnlyList<Food>> GetAllFoodAsync();

    Task<IReadOnlyList<Food>> GetFoodOfCategory(int id);

    Task<bool> ContainsAsync(string foodName);

    Task<int> Register(Food food);
}