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
    [Authorize(Roles = "Admin")]
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
    
    [HttpGet("activity")]
    
    [ProducesResponseType(typeof(UserActivity), 200)]
    public async Task<IActionResult> GetActivityOfUser([FromQuery] UserActivitySpecificationParam param)
    {
        if (!ModelState.IsValid)
        {
            Console.WriteLine(ModelState.Values);
            return BadRequest("The request is not in good shape!");
        }
        
        UserOrderActivitySpecification? orderActivitySpecification = null;
        UserWalletActivitySpecification? walletActivitySpecification = null;

        if (param.Type.HasValue)
        {
            if (param.Type.Value == 1)
            {
                orderActivitySpecification = new UserOrderActivitySpecification(param);
            }
            else if(param.Type == 2)
            {
                walletActivitySpecification = new UserWalletActivitySpecification(param);
            }
            else
            {
                orderActivitySpecification = new UserOrderActivitySpecification(param);
                walletActivitySpecification = new UserWalletActivitySpecification(param);
            }
        }
        else
        {
            orderActivitySpecification = new UserOrderActivitySpecification(param);
            walletActivitySpecification = new UserWalletActivitySpecification(param);
        }

        List<UserActivity> userActivityAsync = await StatisticsRepository.GetUserActivityAsync(orderActivitySpecification, walletActivitySpecification);
        return Ok(userActivityAsync);
    }

    [HttpGet("order-id")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetOrderOfId(string orderId)
    {
        if (!Guid.TryParse(orderId, out Guid result))
        {
            return BadRequest("Failed to parse order id");
        }

        Order? orderOfId = await OrderRepository.GetOrderOfId(result);

        if (orderOfId == null)
        {
            return NotFound();
        }
        
        CompletedOrderView staffCheckOrderDto = Mapper.Map<CompletedOrderView>(orderOfId);

        return Ok(staffCheckOrderDto);
    }

    [HttpGet("revenue")]
    public async Task<IActionResult> GetRevenueOfYear(string yr)
    {
        int year;
        try
        {
            year = Convert.ToInt32(yr);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return BadRequest("An invalid year is provided!");
        }

        List<RevenueReportDto> revenueReportDtos = await StatisticsRepository.GetRevenueAsync(year);
        foreach (RevenueReportDto revenueReportDto in revenueReportDtos)
        {
            Console.WriteLine(revenueReportDto.Month + " "+ revenueReportDto.Revenue);
        }
        return Ok(revenueReportDtos);
    }
}