using Cafet_Backend.Abstracts;
using Cafet_Backend.Dto.Errors;
using Cafet_Backend.Interfaces;
using Cafet_Backend.Models;
using Cafet_Backend.QueryParams;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cafet_Backend.Controllers;

public class CategoryController : AbstractController
{
    private readonly ICategoryRepository CategoryRepository;

    public CategoryController(ICategoryRepository roleRepository)
    {
        CategoryRepository = roleRepository;
        
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        IReadOnlyList<FoodCategory> readOnlyList = await CategoryRepository.GetAllCategoryAsync();
        return Ok(readOnlyList);
    }
    
    
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(FoodCategory), 200)]
    [ProducesResponseType(typeof(ApiException), 404)]
    public async Task<IActionResult> GetById(int id)
    {
        FoodCategory? foodCategory = await CategoryRepository.GetByIdAsync(id);
        if (foodCategory != null) 
            return Ok(foodCategory);
        
        
        return NotFound(new ApiException(404, "The category is unknown", $"The category with name {id} is unknown to the system"));
    }
    
    [HttpGet("name/{name}")]
    [ProducesResponseType(typeof(ApiException), 404)]
    public async Task<IActionResult> GetByName(string name)
    {
        FoodCategory? foodCategory = await CategoryRepository.GetByNameAsync(name);
        if (foodCategory != null)
            return Ok(foodCategory);
        
        
        return NotFound(new ApiException(404, "The category is unknown", $"The category with name {name} is unknown to the system"));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("new")]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ApiResponse), 406)]
    public async Task<IActionResult> RegisterNew([FromBody] CategoryParams categoryParams)
    {
        bool contains = await CategoryRepository.ContainsAsync(categoryParams.CategoryName);
        if (contains)
            return UnprocessableEntity(new ApiException(406, "Duplicate Category Found", $"A category with {categoryParams.CategoryName} already exists in the system!"));

        FoodCategory foodCategory = new FoodCategory()
        {
          CategoryName  = categoryParams.CategoryName,
          CategoryDescription = categoryParams.CategoryDescription,
        };

        await CategoryRepository.RegisterAsync(foodCategory);
        return Ok();
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPost("update")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> Update([FromBody] CategoryUpdateParams categoryParams)
    {
        FoodCategory foodCategory = new FoodCategory()
        {
            Id = categoryParams.id,
            CategoryDescription = categoryParams.categoryDescription,
            CategoryName = categoryParams.categoryName,
        };
        await CategoryRepository.UpdateAsync(foodCategory);
        return Ok();
    }
    
    [Authorize(Roles = "Admin")]
    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await CategoryRepository.DeleteAsync(id);
        return Ok();
    }
}