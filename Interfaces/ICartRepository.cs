using Cafet_Backend.Dto;
using Cafet_Backend.Dto.InputDtos;
using Cafet_Backend.Models;

namespace Cafet_Backend.Interfaces;

public interface ICartRepository
{
    Task<bool> DeleteCart(Guid cartId);

    Task<bool> DeleteCart(Cart cart);

    Task<bool> ClearCart(Guid cartId);

    Task SaveAsync();

    Task<UserCartData?> AddOrUpdateCart(User user, CartAddition cartAddition);

    Task RemoveItemFromUserCart(User user, int foodId);

    Task<CartDto> GetProcessedCartOfUser(User user);
}