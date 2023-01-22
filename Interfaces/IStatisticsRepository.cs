using Cafet_Backend.Dto;

namespace Cafet_Backend.Interfaces;

public interface IStatisticsRepository
{
    Task<bool> Test();

    Task<List<TopSelledFood>> GetTopSoldOfDay(int count);
}