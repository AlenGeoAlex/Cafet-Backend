using Cafet_Backend.Abstracts;
using Cafet_Backend.Dto;
using Cafet_Backend.Dto.InputDtos;
using Cafet_Backend.Interfaces;
using Cafet_Backend.Models;
using Microsoft.AspNetCore.Authorization;
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

    [HttpGet("{id}")]
    public async Task<ActionResult<DailyStockDto>> GetStockOfId(int id)
    {
        DailyStock? stock = await StockRepository.GetStockOfId(id);
        if (stock == null)
            return NoContent();

        return StockRepository.AsDto(stock);
    }
    
    [HttpGet("food/{id}")]
    public async Task<ActionResult<DailyStockDto>> GetStockOfFoodId(int id)
    {
        DailyStock? stock = await StockRepository.GetStockOfFoodId(id);
        if (stock == null)
            return NoContent();

        return StockRepository.AsDto(stock);
    }

    [HttpPost("register")]
    //[Authorize(Roles = "Staff")]
    [ProducesResponseType(typeof(UnprocessableEntityObjectResult), 422)]
    [ProducesResponseType(typeof(OkObjectResult), 200)]
    public async Task<IActionResult> UpdateDailyStock([FromBody] SelectedFood[] selectedFoods, string? clear)
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

        
        bool clearStatus = !(string.IsNullOrEmpty(clear) || clear == "0");
        
        await StockRepository.RegisterDailyStockAsync(dailyStocks, clearStatus);

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

    [HttpPost("update")]
    public async Task<IActionResult> Update([FromBody] DailyStockDto dailyStockDto)
    {
        DailyStock? stock = await StockRepository.GetStockOfId(dailyStockDto.StockId);

        if (stock == null)
            return NoContent();

        stock.CurrentStock = dailyStockDto.CurrentInStock;
        await StockRepository.SaveChangesAsync();
        return Ok();
    }
}