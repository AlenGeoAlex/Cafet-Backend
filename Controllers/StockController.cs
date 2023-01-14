using Cafet_Backend.Abstracts;
using Cafet_Backend.Dto;
using Cafet_Backend.Dto.InputDtos;
using Cafet_Backend.Interfaces;
using Cafet_Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Cafet_Backend.Controllers;

public class StockController : AbstractController
{

    public readonly IStockRepository StockRepository;
    public readonly IFoodRepository FoodRepository;

    public StockController(IStockRepository stockRepository, IFoodRepository foodRepository)
    {
        StockRepository = stockRepository;
        FoodRepository = foodRepository;
    }

    [HttpGet]
    [ProducesResponseType(typeof(DailyStockDto[]), 200)]
    public async Task<ActionResult<List<DailyStockDto>>> GetAllDailyStock()
    {
        List<DailyStock> allStockAsync = await StockRepository.GetAllStockAsync();
        return Ok(StockRepository.AsDto(allStockAsync));
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(UnprocessableEntityObjectResult), 422)]
    [ProducesResponseType(typeof(OkObjectResult), 200)]
    public async Task<IActionResult> UpdateDailyStock([FromBody] SelectedFood[] selectedFoods)
    {
        
        if (selectedFoods == null || selectedFoods.Length == 0)
            return BadRequest("Empty food stock returned!");

        IList<SelectedFood> foods = selectedFoods.ToList();
        List<string> failedtoRegister = new List<string>();
        List<DailyStock> dailyStocks = new List<DailyStock>();
        foreach (SelectedFood selectedFood in foods)
        {
            Food? food = await FoodRepository.GetFoodRawAsync(selectedFood.FoodId);
            if (food == null)
            {
                failedtoRegister.Add(selectedFood.Name);
                continue;
            }

            DailyStock stock = new DailyStock()
            {
                FoodId = food.Id,
                FoodStock = selectedFood.Quantity,
                CurrentStock = selectedFood.Quantity,
            };
            dailyStocks.Add(stock);
        }
        
        await StockRepository.RegisterDailyStockAsync(dailyStocks);

        if (failedtoRegister.Count > 0)
            return UnprocessableEntity(failedtoRegister);
        
        return Ok();
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteStock(int id)
    {
        bool deleted = await StockRepository.DeleteStockAsync(id);
        if (deleted)
            return Ok();
        else return NoContent();
    }
    
    [HttpPost("delete/")]
    public async Task<IActionResult> DeleteStockMultiple([FromBody] List<int> ids)
    {
        bool deleted = await StockRepository.DeleteStockAsync(ids);
        if (deleted)
            return Ok();
        else return NoContent();
    }
}