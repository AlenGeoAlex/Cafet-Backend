using Cafet_Backend.Models;

namespace Cafet_Backend.Interfaces;

public interface ICartRepository
{
    Task<bool> DeleteCart(Guid cartId);

    Task<bool> DeleteCart(Cart cart);

    Task<bool> ClearCart(Guid cartId);

    Task SaveAsync();
}