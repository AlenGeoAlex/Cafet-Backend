using Cafet_Backend.Dto;

namespace Cafet_Backend.Hub;

public interface IStatisticsHubClient
{
    
    Task OnStatisticsUpdate(StatisticsDto statisticsDto);

}