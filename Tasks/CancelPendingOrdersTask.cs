using Cafet_Backend.Interfaces;

namespace Cafet_Backend.Tasks;

public class CancelPendingOrdersTask : IHostedService
{
    public readonly IServiceProvider Services;
    public readonly ILogger<CancelPendingOrdersTask> Logger;

    public Timer? Timer;

    public CancelPendingOrdersTask(IServiceProvider services, ILogger<CancelPendingOrdersTask> logger)
    {
        this.Services = services;
        this.Logger = logger;
    }

    private async void Tick(object? state)
    {
        using (var scope = Services.CreateScope())
        {
            IOrderRepository? repository = scope.ServiceProvider.GetService<IOrderRepository>();

            if (repository == null)
            {
                Logger.LogWarning("Failed to tick failed transactions, The repository was null");
                return;
            }
            Logger.LogInformation("Preparing task to cancel timed-out orders");
            await repository.CancelPendingOrders();
        }
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (Timer != null)
            return Task.CompletedTask;
        
        this.Timer = new Timer(
            Tick,
            null,
            TimeSpan.FromSeconds(6),
            TimeSpan.FromMinutes(1)
        );

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }
    
}