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

        CreateMap<User, UserDto>()
            .ForMember(dto => dto.UserEmailAddress, o => o.MapFrom(user => user.EmailAddress))
            .ForMember(dto => dto.UserFullName, o => o.MapFrom(user => user.FullName))
            .ForMember(dto => dto.UserRole, o => o.MapFrom(user => user.Role.RoleName))
            .ForMember(dto => dto.CartId, o => o.MapFrom(user => user.Cart.CartId))
            .ForMember(dto => dto.ImageLink, o => o.MapFrom<UserImageUrlResolver>());

    }
}