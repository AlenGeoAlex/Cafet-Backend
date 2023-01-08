using Cafet_Backend.Dto;
using Cafet_Backend.Models;

namespace Cafet_Backend.Interfaces;

public interface IFoodRepository
{
    Task<FoodDto?> GetFoodByIdAsync(int id);

    Task<Food?> GetFoodRawAsync(int id);

    Task<FoodDto?> GetFoodByNameAsync(string name);

    Task<IReadOnlyList<FoodDto>> GetMatchingFoodOfNamesAsync(string keyword);

    Task<IReadOnlyList<FoodDto>> GetAllFoodAsync();

    Task<IReadOnlyList<FoodDto>> GetFoodOfCategory(int id);

    Task<bool> ContainsAsync(string foodName);

    Task<int> Register(Food? food);

    FoodDto AsDto(Food? food);

    Task<int> Delete(int id);

    IReadOnlyList<FoodDto> AsDtoList(IReadOnlyList<Food> foods);

    Task<int> Update(Food food);

}