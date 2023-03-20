using Cafet_Backend.Dto;
using Cafet_Backend.Models;
using Cafet_Backend.Specification;

namespace Cafet_Backend.Interfaces;

public interface IStatisticsRepository
{
    Task<int> GetTotalOrderCount();

    Task<int> GetUserCount();
    Task<int> GetOrderCountBy(Specification<Order> specification);
    Task<double> GetCostReturn(Specification<Order> specification);
    Task<List<TopSelledFood>> GetTopSoldOfDay(int count);
    Task<StatisticsDto> GetCurrentStatistics();
    Task<List<UserActivity>> GetUserActivityAsync(UserOrderActivitySpecification? orderActivitySpecification, UserWalletActivitySpecification? walletActivitySpecification);

    Task<List<RevenueReportDto>> GetRevenueAsync(int year);
}