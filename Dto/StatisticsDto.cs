namespace Cafet_Backend.Dto;

public class StatisticsDto
{
    public long UserCount { get; set; }
    public long TotalOrders { get; set; }
    public long CompletedOrders { get; set; }
    public double CostEarned { get; set; }
}