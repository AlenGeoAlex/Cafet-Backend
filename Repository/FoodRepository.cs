using AutoMapper;
using Cafet_Backend.Context;
using Cafet_Backend.Dto;
using Cafet_Backend.Interfaces;
using Cafet_Backend.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Cafet_Backend.Repository;

public class FoodRepository : IFoodRepository
{

    private readonly CafeContext CafeContext;
    private readonly IMapper Mapper;
    public FoodRepository(CafeContext cafeContext, IMapper mapper)
    {
        CafeContext = cafeContext;
        Mapper = mapper;
    }

    public async Task<FoodDto?> GetFoodByIdAsync(int id)
    {
        Food? food = await CafeContext.Foods
            .Include(food => food.Category)
            .FirstOrDefaultAsync(food => food.Id == id);

        return AsDto(food);
    }

    public async Task<Food?> GetFoodRawAsync(int id)
    {
        return  await CafeContext.Foods
            .Include(food => food.Category)
            .FirstOrDefaultAsync(food => food.Id == id);
        ;
    }

    public async Task<FoodDto?> GetFoodByNameAsync(string name)
    {
        Food? food = await CafeContext.Foods
            .Include(food => food.Category)
            .FirstOrDefaultAsync(f => f.Name == name);
        
        return AsDto(food);

    }

    public async Task<IReadOnlyList<FoodDto>> GetMatchingFoodOfNamesAsync(string keyword)
    {
        IReadOnlyList<Food> foods =  await CafeContext.Foods
            .Include(food => food.Category)
            .Where(f => f.Name.Contains(keyword)).ToListAsync();

        return AsDtoList(foods);

    }

    public async Task<IReadOnlyList<FoodDto>> GetAllFoodAsync()
    {
        IReadOnlyList<Food> foods =  await CafeContext.Foods
            .Include(food => food.Category)
            .ToListAsync();
        
        return AsDtoList(foods);
    }

    public async Task<IReadOnlyList<FoodDto>> GetFoodOfCategory(int id)
    {
        IReadOnlyList<Food> foods = await CafeContext.Foods.Where(f => f.CategoryId == id).ToListAsync();
        return AsDtoList(foods);
    }

    public Task<bool> ContainsAsync(string foodName)
    {
        throw new NotImplementedException();
    }

    public Task<int> Register(Food? food)
    {
        CafeContext.Foods.AddAsync(food);
        return CafeContext.SaveChangesAsync();
    }

    public FoodDto AsDto(Food? food)
    {
        return Mapper.Map<Food, FoodDto>(food);
    }

    public async Task<int> Delete(int id)
    {
        Food? findAsync = await CafeContext.Foods.FindAsync(id);
        if (findAsync == null)
            return 0;

        CafeContext.Foods.Remove(findAsync);
        return await CafeContext.SaveChangesAsync();
    }

    public IReadOnlyList<FoodDto> AsDtoList(IReadOnlyList<Food> foods)
    {
        List<FoodDto> returnList = new List<FoodDto>();
        foreach (Food? food in foods)
        {
            returnList.Add(AsDto(food));
        }
        return returnList;
    }

    public async Task<int> Update(Food food)
    {
        CafeContext.Foods.Update(food);
        return await CafeContext.SaveChangesAsync();
    }
}