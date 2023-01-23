using AutoMapper;
using Cafet_Backend.Abstracts;
using Cafet_Backend.Dto;
using Cafet_Backend.Interfaces;
using Cafet_Backend.Models;
using Cafet_Backend.Specification;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace Cafet_Backend.Controllers;

public class SearchController : AbstractController
{
    public static readonly IReadOnlyList<EmailQueryDto> EmptyEmailList = new List<EmailQueryDto>();
    public static readonly IReadOnlyList<DailyStockDto> EmptyStockList = new List<DailyStockDto>();
    
    private readonly IUserRepository UserRepository;
    private readonly IFoodRepository FoodRepository;
    private readonly IStockRepository StockRepository;
    private readonly IOrderRepository OrderRepository;
    private readonly IMapper Mapper;

    public SearchController(IUserRepository userRepository, IFoodRepository foodRepository, IStockRepository stockRepository, IOrderRepository orderRepository, IMapper mapper)
    {
        UserRepository = userRepository;
        FoodRepository = foodRepository;
        StockRepository = stockRepository;
        OrderRepository = orderRepository;
        Mapper = mapper;
    }

    [HttpGet("order")]
    [ProducesResponseType(typeof(List<StaffCheckOrderDto>), 200)]
    public async Task<ActionResult<List<StaffCheckOrderDto>>> GetOrder([FromQuery] OrderSearchSpecificationParam param)
    {
        OrderSearchSpecification specification = new OrderSearchSpecification(param);
        List<Order> searchOrderFor = await OrderRepository.GetOrdersFor(specification);
        return Ok(Mapper.Map<List<StaffCheckOrderDto>>(searchOrderFor));
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