using AutoMapper;
using Cafet_Backend.Dto;
using Cafet_Backend.Models;

namespace Cafet_Backend.Helper;

public class FoodUrlResolver : IValueResolver<Food, FoodDto, string>
{
    
    private readonly IConfiguration Config;
    
    public FoodUrlResolver(IConfiguration configuration)
    {
        this.Config = configuration;
    }
    
    public string Resolve(Food source, FoodDto destination, string destMember, ResolutionContext context)
    {
        if (string.IsNullOrEmpty(source.FoodImage))
            return null;

        
        return $"{Config["apiUrl"]}_images/_food/{source.FoodImage}";
    }
}