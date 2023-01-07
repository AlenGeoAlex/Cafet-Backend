using Cafet_Backend.Context;
using Cafet_Backend.Interfaces;
using Cafet_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Cafet_Backend.Repository;

public class FoodRepository : IFoodRepository
{

    private readonly CafeContext CafeContext;

    public FoodRepository(CafeContext cafeContext)
    {
        CafeContext = cafeContext;
    }

    public async Task<Food?> GetFoodByIdAsync(int id)
    {
        return await CafeContext.Foods
            .Include(food => food.Category)
            .FirstOrDefaultAsync(food => food.Id == id);
    }

    public async Task<Food?> GetFoodByNameAsync(string name)
    {
        return await CafeContext.Foods
            .Include(food => food.Category)
            .FirstOrDefaultAsync(f => f.Name == name);
    }

    public async Task<IReadOnlyList<Food>> GetMatchingFoodOfNamesAsync(string keyword)
    {
        return await CafeContext.Foods
            .Include(food => food.Category)
            .Where(f => f.Name.Contains(keyword)).ToListAsync();

    }

    public async Task<IReadOnlyList<Food>> GetAllFoodAsync()
    {
        return await CafeContext.Foods
            .Include(food => food.Category)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Food>> GetFoodOfCategory(int id)
    {
        return await CafeContext.Foods.Where(f => f.CategoryId == id).ToListAsync();
    }

    public Task<bool> ContainsAsync(string foodName)
    {
        throw new NotImplementedException();
    }

    public Task<int> Register(Food food)
    {
        CafeContext.Foods.AddAsync(food);
        return CafeContext.SaveChangesAsync();
    }
}