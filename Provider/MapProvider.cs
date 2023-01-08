using AutoMapper;
using Cafet_Backend.Dto;
using Cafet_Backend.Helper;
using Cafet_Backend.Models;
using Cafet_Backend.Provider.Image;

namespace Cafet_Backend.Provider;

public class MapProvider : Profile
{
    public MapProvider()
    {
        CreateMap<Food, FoodDto>()
            .ForMember(dto => dto.FoodId, o => o.MapFrom(food => food.Id) )
            .ForMember(dto => dto.Category, food => food.MapFrom(food => food.Category.CategoryName))
            .ForMember(dto => dto.FoodImage, food => food.MapFrom<FoodUrlResolver>());
    }
}