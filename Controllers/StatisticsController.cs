using AutoMapper;
using Cafet_Backend.Abstracts;
using Cafet_Backend.Dto;
using Cafet_Backend.Interfaces;
using Cafet_Backend.Models;
using Cafet_Backend.Repository;
using Cafet_Backend.Specification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cafet_Backend.Controllers;

public class StatisticsController : AbstractController
{

    public readonly IStatisticsRepository StatisticsRepository;
    public readonly ILogger<StatisticsRepository> Logger;
    public readonly IOrderRepository OrderRepository;
    public readonly IMapper Mapper;

    public StatisticsController(IStatisticsRepository statisticsRepository, IOrderRepository orderRepository, IMapper mapper, ILogger<StatisticsRepository> logger)
    {
        StatisticsRepository = statisticsRepository;
        OrderRepository = orderRepository;
        Mapper = mapper;
        Logger = logger;
    }

    [HttpGet("top-seller")]
    [ProducesResponseType( typeof(List<TopSelledFood>), 200)]
    public async Task<ActionResult<List<TopSelledFood>>> GetTopSoldOfDay(int? count)
    {
        int toCount = 3;
        if (count is > 0)
        {
            toCount = count.Value;
        }

        return await StatisticsRepository.GetTopSoldOfDay(toCount);
    }

    [HttpGet("order")]
    // [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(StaffCheckOrderDto), 200)]
    public async Task<IActionResult> GetOrderBySpecification([FromQuery] OrderReportSpecificationParam specificationParam)
    {
        if (specificationParam == null)
        {
            specificationParam = new OrderReportSpecificationParam()
            {
                PaymentMethod = 0,
                OnlyCompleted = false,
            };
        }

        OrderReportSpecification specification = new OrderReportSpecification(specificationParam);
        List<Order> ordersFor = await OrderRepository.GetOrdersFor(specification);
        List<StaffCheckOrderDto> processedOrder = Mapper.Map<List<StaffCheckOrderDto>>(ordersFor);
        return Ok(processedOrder);
    }
}