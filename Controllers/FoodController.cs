using Cafet_Backend.Abstracts;
using Cafet_Backend.Dto.Errors;
using Cafet_Backend.Interfaces;
using Cafet_Backend.Manager;
using Cafet_Backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cafet_Backend.Controllers;

public class FoodController : AbstractController
{
    private readonly IFoodRepository FoodRepository;
    private readonly ImageProviderManager ImageProviderManager;
    
    public FoodController(IFoodRepository foodRepository, ImageProviderManager imageProviderManager)
    {
        FoodRepository = foodRepository;
        ImageProviderManager = imageProviderManager;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        IReadOnlyList<Food> readOnlyList = await FoodRepository.GetAllFoodAsync();
        return Ok(readOnlyList);
    }
    
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiException), 404)]
    public async Task<IActionResult> GetById(int id)
    {
        Food? food = await FoodRepository.GetFoodByIdAsync(id);
        if (food != null)
            Ok(food);
        
        
        return NotFound(new ApiException(404, "The food is unknown", $"The food with name {id} is unknown to the system"));
    }

    [HttpGet("name/{name}")]
    [ProducesResponseType(typeof(ApiException), 404)]
    public async Task<IActionResult> GetByName(string name)
    {
        Food? food = await FoodRepository.GetFoodByNameAsync(name);
        if (food != null)
            Ok(food);
        
        
        return NotFound(new ApiException(404, "The food is unknown", $"The food with name {name} is unknown to the system"));
    }
    
    
    [HttpPost("new")]
    [RequestFormLimits(ValueCountLimit = Int32.MaxValue)]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ApiResponse), 406)]
    public async Task<IActionResult> RegisterNew()
    {
        IFormCollection readFormAsync = await Request.ReadFormAsync();

        string FoodName = readFormAsync["FoodName"];
        string CategoryName = readFormAsync["CategoryName"];
        string CategoryId = readFormAsync["CategoryId"];
        string FoodDescription = readFormAsync["FoodDescription"];
        string FoodPrice = readFormAsync["FoodPrice"];
        string ImageName = "default.png";
        
        IFormFile? @default = readFormAsync.Files.FirstOrDefault();
        if (@default != null)
        {
            string fileName = $"{Guid.NewGuid().ToString().Replace("-", "")}-{@default.FileName}";
            string fullFile = ImageProviderManager.FoodImageProvider.AsFileName(fileName);
            using (FileStream fStream = new FileStream(fullFile, FileMode.Create))
            {
                await @default.CopyToAsync(fStream);
                Console.WriteLine("File uploaded "+fullFile);
                ImageName = fileName;
            }
        }

        Food newFood = new Food()
        {
            Name = FoodName,
            CategoryId = Convert.ToInt32(CategoryId),
            FoodDescription = FoodDescription,
            FoodImage = ImageName,
            FoodPrice = Convert.ToDouble(FoodPrice)
        };

        await FoodRepository.Register(newFood);
        
        return Ok();
    }
}