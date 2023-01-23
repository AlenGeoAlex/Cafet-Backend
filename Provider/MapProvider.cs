using AutoMapper;
using Cafet_Backend.Dto;
using Cafet_Backend.Helper;
using Cafet_Backend.Helper.Resolver;
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
            .ForMember(dto => dto.Vegetarian, o => o.MapFrom(food => food.Vegetarian))
            .ForMember(dto => dto.Tags, o => o.MapFrom(food => food.TagCollection))
            .ForMember(dto => dto.Category, food => food.MapFrom(food => food.Category.CategoryName))
            .ForMember(dto => dto.FoodImage, food => food.MapFrom<FoodUrlResolver>());

        CreateMap<User, CredentialsDto>()
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
            .ForMember(dto => dto.PhoneNumber, o => o.MapFrom(user => user.PhoneNumber))
            .ForMember(dto => dto.Activated, o => o.MapFrom(user => user.Activated))
            .ForMember(dto => dto.Deleted, o => o.MapFrom(user => user.Deleted));


        CreateMap<DailyStock, DailyStockDto>()
            .ForMember(dto => dto.FoodPrice, o => o.MapFrom(ds => ds.Food.FoodPrice))
            .ForMember(dto => dto.FoodImage, o => o.MapFrom<DailyStockImageUrlResolver>())
            .ForMember(dto => dto.FoodId, o => o.MapFrom(ds => ds.Food.Id))
            .ForMember(dto => dto.FoodCategory, o => o.MapFrom(ds => ds.Food.Category.CategoryName))
            .ForMember(dto => dto.FoodName, o => o.MapFrom(ds => ds.Food.Name))
            .ForMember(dto => dto.StockId, o => o.MapFrom(ds => ds.Id))
            .ForMember(dto => dto.CurrentInStock, o => o.MapFrom(ds => ds.CurrentStock))
            .ForMember(dto => dto.FoodType, o => o.MapFrom(ds => ds.Food.Vegetarian))
            .ForMember(dto => dto.FoodDescription, o => o.MapFrom(ds => ds.Food.FoodDescription))
            .ForMember(dto => dto.TotalInStock, o => o.MapFrom(ds => ds.FoodStock));

        CreateMap<Order, StaffCheckOrderDto>()
            .ForMember(dto => dto.OrderId, o => o.MapFrom(d => d.Id))
            .ForMember(dto => dto.OrderedEmail, o => o.MapFrom(d => d.OrderPlacedFor.EmailAddress))
            .ForMember(dto => dto.OrderedDate, o => o.MapFrom(d => d.OrderPlaced.Date.ToShortDateString()))
            .ForMember(dto => dto.OrderedTime, o => o.MapFrom(d => d.OrderPlaced.ToShortTimeString()))
            .ForMember(dto => dto.OrderedUserName, o => o.MapFrom(d => d.OrderPlacedFor.FullName))
            .ForMember(dto => dto.IsCompleted, o => o.MapFrom(d => d.OrderDelivered != null))
            .ForMember(dto => dto.IsCancelled, o => o.MapFrom(d => d.Cancelled))
            .ForMember(dto => dto.OrderedFoods, o => o.MapFrom(d => d.OrderItems))
            ;

        CreateMap<OrderItems, StaffCheckFoodDto>()
            .ForMember(dto => dto.FoodId, o => o.MapFrom(s => s.Food.Id))
            .ForMember(dto => dto.FoodName, o => o.MapFrom(d => d.Food.Name))
            .ForMember(dto => dto.FoodCategory, o => o.MapFrom(d => d.Food.Category.CategoryName))
            .ForMember(dto => dto.FoodType, o => o.MapFrom(d => d.Food.Vegetarian))
            .ForMember(dto => dto.FoodPrice, o => o.MapFrom(d => d.Food.FoodPrice))
            .ForMember(dto => dto.FoodQuantity, o => o.MapFrom(d => d.Quantity))
            .ForMember(dto => dto.FoodImageUrl, o => o.MapFrom<StaffCheckOrderImageResolver>());
            

    }
}