using Cafet_Backend.Context;
using Cafet_Backend.Dto;
using Cafet_Backend.Interfaces;
using Cafet_Backend.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Cafet_Backend.Repository;

public class CartRepository : ICartRepository
{

    private readonly CafeContext CafeContext;

    public CartRepository(CafeContext cafeContext)
    {
        CafeContext = cafeContext;
    }

    public async Task<bool> DeleteCart(Guid cartId)
    {
        Cart? cartOrDefault = await CafeContext.Carts.FirstOrDefaultAsync(cart => cart.CartId == cartId);
        if (cartOrDefault == null)
            return false;

        return await DeleteCart(cartOrDefault);
    }

    public async Task<bool> DeleteCart(Cart cart)
    {
        CafeContext.Remove(cart);
        await SaveAsync();
        return true;    
    }

    public async Task<bool> ClearCart(Guid cartId)
    {
        Cart? cardById = await GetCart(cartId);

        if (cardById == null)
            return false;
        
        cardById.FoodCartData.Clear();
        CafeContext.Carts.Update(cardById);
        await SaveAsync();
        return true;
    }

    public async Task SaveAsync()
    {
        await CafeContext.SaveChangesAsync();
    }

    public async Task<List<CartDto>> GetProcessedCart(Guid cartId)
    {
        List<CartDto> cartDtos = new List<CartDto>();

        Cart? cart = await GetCart(cartId);

        if (cart == null)
            return cartDtos;
        
        
        return cartDtos;
    }

    public async Task<Cart?> GetCart(Guid cartId)
    {
        return await CafeContext.Carts.FirstOrDefaultAsync(cart => cart.CartId == cartId);
    }
}