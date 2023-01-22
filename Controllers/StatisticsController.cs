using Cafet_Backend.Abstracts;
using Cafet_Backend.Dto;
using Cafet_Backend.Interfaces;
using Cafet_Backend.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Cafet_Backend.Controllers;

public class StatisticsController : AbstractController
{

    public readonly IStatisticsRepository StatisticsRepository;
    public readonly ILogger<StatisticsRepository> Logger;

    public StatisticsController(IStatisticsRepository statisticsRepository, ILogger<StatisticsRepository> logger)
    {
        StatisticsRepository = statisticsRepository;
        Logger = logger;
    }

    /*[HttpGet]
    public async void onGet()
    {
        await StatisticsRepository.Test();
    }*/

    [HttpGet("top-seller")]
    [ProducesResponseType( typeof(List<TopSelledFood>), 200)]
    public async Task<ActionResult<List<TopSelledFood>>> GetTopSoldOfDay(int? count)
    {
        int toCount = 3;
        if (count.HasValue && count.Value > 0)
        {
            toCount = count.Value;
        }

        return await StatisticsRepository.GetTopSoldOfDay(toCount);
    }
}