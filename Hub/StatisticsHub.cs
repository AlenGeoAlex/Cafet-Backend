using Cafet_Backend.Dto;
using Cafet_Backend.Interfaces;
using Cafet_Backend.Models;
using Cafet_Backend.Specification;
using Microsoft.AspNetCore.SignalR;

namespace Cafet_Backend.Hub;

public class StatisticsHub : Hub<IStatisticsHubClient>
{
    public readonly ILogger<StatisticsHub> Logger;
    public readonly IStatisticsRepository StatisticsRepository;

    public StatisticsHub(ILogger<StatisticsHub> logger, IStatisticsRepository statisticsRepository)
    {
        Logger = logger;
        StatisticsRepository = statisticsRepository;
    }
    
    public override Task OnConnectedAsync()
    {
        StatisticsUserHandler.UsersConnected.Add(Context.ConnectionId);
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
        StatisticsUserHandler.UsersConnected.Remove(Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }

    public async Task UpdateStats()
    {
        if(StatisticsUserHandler.UsersConnected.Count <= 0)
            return;

        StatisticsDto currentStatistics = await StatisticsRepository.GetCurrentStatistics();
        await Clients.All.OnStatisticsUpdate(currentStatistics);
    }

    public async Task FetchStats()
    {
        StatisticsDto currentStatistics = await StatisticsRepository.GetCurrentStatistics();
        await Clients.Caller.OnStatisticsUpdate(currentStatistics);
    }


}

public static class StatisticsUserHandler
{
    public static readonly HashSet<string> UsersConnected = new HashSet<string>();
}