using Cafet_Backend.Context;
using Cafet_Backend.Interfaces;
using Cafet_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Cafet_Backend.Repository;

public class CategoryRepository : ICategoryRepository
{

    private readonly CafeContext CafeContext;

    public CategoryRepository(CafeContext cafeContext)
    {
        CafeContext = cafeContext;
    }
    
    public async Task<FoodCategory?> GetByIdAsync(int id)
    {
        return await CafeContext.Categories.FindAsync(id);
    }

    public async Task<FoodCategory?> GetByNameAsync(string name)
    {
        return await CafeContext.Categories.FirstOrDefaultAsync(cName => cName != null && cName.CategoryName == name);
    }

    public async Task<List<FoodCategory?>> GetAllCategoryAsync()
    {
        return await CafeContext.Categories.ToListAsync();
    }

    public async Task<bool> ContainsAsync(string name)
    {
        return await CafeContext.Categories.Where(c => c.CategoryName != null && c.CategoryName == name).CountAsync() >
               0;
    }

    public async Task<int> RegisterAsync(FoodCategory category)
    {
        await CafeContext.Categories.AddAsync(category);
        return await CafeContext.SaveChangesAsync();
    }

    public async Task<int> UpdateAsync(FoodCategory category)
    {
        CafeContext.Categories.Update(category);
        return await CafeContext.SaveChangesAsync();
    }

    public async Task<int> DeleteAsync(int id)
    {
        FoodCategory? byIdAsync = await GetByIdAsync(id);
        if (byIdAsync == null)
        {
            return 0;
        }

        CafeContext.Categories.Remove(byIdAsync);
        return await CafeContext.SaveChangesAsync();

    }
}