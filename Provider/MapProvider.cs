using AutoMapper;
using Cafet_Backend.Dto;
using Cafet_Backend.Helper;
using Cafet_Backend.Models;
using Cafet_Backend.Provider.Image;
using Newtonsoft.Json;

namespace Cafet_Backend.Provider;

public class MapProvider : Profile
{
    public MapProvider()
    {
        CreateMap<Food, FoodDto>()
            .ForMember(dto => dto.FoodId, o => o.MapFrom(food => food.Id) )
            .ForMember(dto => dto.Category, food => food.MapFrom(food => food.Category.CategoryName))
            .ForMember(dto => dto.FoodImage, food => food.MapFrom<FoodUrlResolver>());

        CreateMap<User, CredentialsDto>()
            .ForMember(dto => dto.CartData, o => o.MapFrom(user => JsonConvert.SerializeObject(user.Cart.CartData)))
            .ForMember(dto => dto.UserEmailAddress, o => o.MapFrom(user => user.EmailAddress))
            .ForMember(dto => dto.UserFullName, o => o.MapFrom(user => user.FullName))
            .ForMember(dto => dto.UserRole, o => o.MapFrom(user => user.Role.RoleName))
            .ForMember(dto => dto.CartId, o => o.MapFrom(user => user.Cart.CartId))
            .ForMember(dto => dto.ImageLink, o => o.MapFrom<CredentialsImageUrlResolver>());

        CreateMap<User, UserDto>()
            .ForMember(dto => dto.UserId, o => o.MapFrom(user => user.Id))
            .ForMember(dto => dto.UserFirstName, o => o.MapFrom(user => user.FirstName))
            .ForMember(dto => dto.UserLastName, o => o.MapFrom(user => user.LastName))
            .ForMember(dto => dto.UserEmail, o => o.MapFrom(user => user.EmailAddress))
            .ForMember(dto => dto.UserName, o => o.MapFrom(user => user.FullName))
            .ForMember(dto => dto.UserRole, o => o.MapFrom(user => user.Role.RoleName))
            .ForMember(dto => dto.UserImage, o => o.MapFrom<UserImageUrlResolver>())
            .ForMember(dto => dto.WalletBalance, o => o.MapFrom(user => user.WalletBalance))
            .ForMember(dto => dto.Activated, o => o.MapFrom(user => user.Activated))
            .ForMember(dto => dto.Deleted, o => o.MapFrom(user => user.Deleted));
            
        

        

    }
}