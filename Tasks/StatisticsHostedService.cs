using Cafet_Backend.Dto;
using Cafet_Backend.Hub;
using Cafet_Backend.Interfaces;
using Cafet_Backend.Models;
using Cafet_Backend.Repository;
using Cafet_Backend.Specification;
using Microsoft.AspNetCore.SignalR;

namespace Cafet_Backend.Tasks;

public class StatisticsHostService : BackgroundService
{
    
    private readonly ILogger<StatisticsHostService> Logger;
    private Timer? Timer = null;
    private IServiceProvider ServiceProvider;

    public StatisticsHostService(ILogger<StatisticsHostService> logger,  IServiceProvider serviceProvider)
    {
        Logger = logger;
        ServiceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
         await Execute(stoppingToken);
    }
    
    

    private async Task Execute(CancellationToken stoppingToken)
    {
        using (var scope = ServiceProvider.CreateScope())
        {
            var scopedProcessingService = 
                scope.ServiceProvider
                    .GetRequiredService<IScopedProcessingService>();

            await scopedProcessingService.DoWork(stoppingToken);
        }
    }
    
    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        Logger.LogInformation(
            "Consume Scoped Service Hosted Service is stopping.");

        await base.StopAsync(stoppingToken);
    }
    
}

internal interface IScopedProcessingService
{
    Task DoWork(CancellationToken stoppingToken);
}

internal class ScopedStatisticsService : IScopedProcessingService
{
    private readonly ILogger Logger;
    public readonly IHubContext<StatisticsHub, IStatisticsHubClient> StatisticsHub;
    private readonly IStatisticsRepository StatisticsRepository;
    public ScopedStatisticsService(ILogger<ScopedStatisticsService> logger, IHubContext<StatisticsHub, IStatisticsHubClient> statisticsHub, IStatisticsRepository statisticsRepository)
    {
        Logger = logger;
        StatisticsHub = statisticsHub;
        StatisticsRepository = statisticsRepository;
    }

    public async Task DoWork(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await StatisticsHub.Clients.All.OnStatisticsUpdate(await StatisticsRepository.GetCurrentStatistics());
            
            await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
        }
    }
}