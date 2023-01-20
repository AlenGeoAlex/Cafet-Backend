using AutoMapper;
using Cafet_Backend.Abstracts;
using Cafet_Backend.Dto;
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
    private readonly IMapper Mapper;
    public FoodController(IFoodRepository foodRepository, ImageProviderManager imageProviderManager, IMapper mapper)
    {
        FoodRepository = foodRepository;
        ImageProviderManager = imageProviderManager;
        Mapper = mapper;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        IReadOnlyList<FoodDto> readOnlyList = await FoodRepository.GetAllFoodAsync();
        return Ok(readOnlyList);
    }
    
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiException), 404)]
    public async Task<IActionResult> GetById(int id)
    {
        FoodDto? food = await FoodRepository.GetFoodByIdAsync(id);
        if (food != null) 
            return Ok(food);
        
        
        return NotFound(new ApiException(404, "The food is unknown", $"The food with name {id} is unknown to the system"));
    }

    [HttpGet("name/{name}")]
    [ProducesResponseType(typeof(ApiException), 404)]
    public async Task<IActionResult> GetByName(string name)
    {
        FoodDto? food = await FoodRepository.GetFoodByNameAsync(name);
        if (food != null)
            return Ok(food);
        
        
        return NotFound(new ApiException(404, "The food is unknown", $"The food with name {name} is unknown to the system"));
    }
    
    [HttpPost("update")]
    [RequestFormLimits(ValueCountLimit = Int32.MaxValue)]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ApiResponse), 406)]
    public async Task<IActionResult> Update()
    {   
        IFormCollection readFormAsync = await Request.ReadFormAsync();
        int FoodId = Convert.ToInt32(readFormAsync["FoodId"]);

        Food? foodRawAsync = await FoodRepository.GetFoodRawAsync(FoodId);
        if (foodRawAsync == null)
            return NotFound();

        string oldImage = foodRawAsync.FoodImage;

        string FoodName = readFormAsync["FoodName"];
        string CategoryName = readFormAsync["CategoryName"];
        string CategoryId = readFormAsync["CategoryId"];
        string FoodDescription = readFormAsync["FoodDescription"];
        string FoodPrice = readFormAsync["FoodPrice"];
        string ImageName = foodRawAsync.FoodImage;
        string V = readFormAsync["FoodType"];
        List<string> Tags = new List<string>();

        bool Vegeterian = Convert.ToBoolean(V);
        string _tags = readFormAsync["Tags"];
        string[] tagArray = _tags.Split(",");
        if (tagArray != null && tagArray.Length > 0)
        {
            foreach (string tag in tagArray)
            {
                Tags.Add(tag);
            }
        }
        
        IFormFile? @default = readFormAsync.Files.FirstOrDefault();
        if (@default != null)
        {
            string fileName = $"{Guid.NewGuid().ToString().Replace("-", "")}-{@default.FileName.Replace("-", "")}";
            fileName = fileName.Trim();
            string fullFile = ImageProviderManager.FoodImageProvider.AsFileName(fileName);
            using (FileStream fStream = new FileStream(fullFile, FileMode.Create))
            {
                await @default.CopyToAsync(fStream);
                Console.WriteLine("File uploaded "+fullFile);
                ImageName = fileName;
            }
        }

        Food? newFood = new Food()
        {
            Name = FoodName,
            Id = FoodId,
            CategoryId = Convert.ToInt32(CategoryId),
            FoodDescription = FoodDescription,
            FoodImage = ImageName,
            FoodPrice = Convert.ToDouble(FoodPrice),
            TagCollection = Tags,
            Vegetarian = Vegeterian 
        };

        await FoodRepository.Update(newFood);
        if (ImageName != "default.png")
        {
            ImageProviderManager.FoodImageProvider.Delete(oldImage);
        }
        return Ok();
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
        string V = readFormAsync["FoodType"];
        List<string> Tags = new List<string>();

        bool Vegeterian = Convert.ToBoolean(V);
        string _tags = readFormAsync["Tags"];
        string[] tagArray = _tags.Split(",");
        if (tagArray != null && tagArray.Length > 0)
        {
            foreach (string tag in tagArray)
            {
                Tags.Add(tag);
            }
        }

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

        Food? newFood = new Food()
        {
            Name = FoodName,
            CategoryId = Convert.ToInt32(CategoryId),
            FoodDescription = FoodDescription,
            FoodImage = ImageName,
            FoodPrice = Convert.ToDouble(FoodPrice),
            Vegetarian = Vegeterian,
            TagCollection = Tags,
        };

        await FoodRepository.Register(newFood);
        
        return Ok();
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await FoodRepository.Delete(id);

        return Ok();
    }
}