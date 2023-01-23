using AutoMapper;
using Cafet_Backend.Dto;
using Cafet_Backend.Models;

namespace Cafet_Backend.Helper.Resolver;

public class StaffCheckOrderImageResolver : IValueResolver<OrderItems, StaffCheckFoodDto, string>
{
    private readonly IConfiguration Config;
    
    public StaffCheckOrderImageResolver(IConfiguration configuration)
    {
        this.Config = configuration;
    }
    
    public string Resolve(OrderItems source, StaffCheckFoodDto destination, string destMember, ResolutionContext context)
    {
        if (string.IsNullOrEmpty(source.Food.FoodImage))
            return null;

        
        return $"{Config["apiUrl"]}_images/_food/{source.Food.FoodImage}";
    }
}