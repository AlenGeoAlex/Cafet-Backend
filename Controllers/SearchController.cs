using Cafet_Backend.Abstracts;
using Cafet_Backend.Dto;
using Cafet_Backend.Interfaces;
using Cafet_Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Cafet_Backend.Controllers;

public class SearchController : AbstractController
{
    public static readonly IReadOnlyList<EmailQueryDto> EmptyEmailList = new List<EmailQueryDto>();
    public static readonly IReadOnlyList<DailyStockDto> EmptyStockList = new List<DailyStockDto>();
    
    private readonly IUserRepository UserRepository;
    private readonly IFoodRepository FoodRepository;
    private readonly IStockRepository StockRepository;

    public SearchController(IUserRepository userRepository, IFoodRepository foodRepository, IStockRepository stockRepository)
    {
        UserRepository = userRepository;
        FoodRepository = foodRepository;
        StockRepository = stockRepository;
    }

    [HttpGet("users")]
    public async Task<IReadOnlyList<EmailQueryDto>> GetEmailAddress(string queryString)
    {
        if (queryString.Length <= 3)
            return EmptyEmailList;

        return await UserRepository.GetEmailAddress(queryString);
    }
    
    [HttpGet("user")]
    public async Task<ActionResult<EmailQueryDto>> GetUserEmailAddress(string queryString)
    {
        EmailQueryDto? emailQueryDto = await UserRepository.GetUserOfEmailAddress(queryString);
        if (emailQueryDto == null)
            return NoContent();

        return emailQueryDto;
    }
    
    [HttpGet("stocks")]
    public async Task<IReadOnlyList<DailyStockDto>> GetStock(string queryString)
    {
        if (queryString.Length <= 2)
            return EmptyStockList;

        List<DailyStock> stockFromFoodName = await StockRepository.GetStockFromFoodName(queryString);
        
        List<DailyStockDto> dailyStockDtos = StockRepository.AsDto(stockFromFoodName);

        return dailyStockDtos;
    }
}