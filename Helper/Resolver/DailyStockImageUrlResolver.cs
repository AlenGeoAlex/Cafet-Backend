using AutoMapper;
using Cafet_Backend.Dto;
using Cafet_Backend.Models;

namespace Cafet_Backend.Helper;

public class DailyStockImageUrlResolver : IValueResolver<DailyStock, DailyStockDto, string>
{
    private readonly IConfiguration Config;
    
    public DailyStockImageUrlResolver(IConfiguration configuration)
    {
        this.Config = configuration;
    }
    
    public string Resolve(DailyStock source, DailyStockDto destination, string destMember, ResolutionContext context)
    {
        if (string.IsNullOrEmpty(source.Food.FoodImage))
            return null;

        
        return $"{Config["apiUrl"]}_images/_food/{source.Food.FoodImage}";
    }
}