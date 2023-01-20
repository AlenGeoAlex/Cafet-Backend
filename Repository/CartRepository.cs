using Cafet_Backend.Context;
using Cafet_Backend.Dto;
using Cafet_Backend.Dto.InputDtos;
using Cafet_Backend.Interfaces;
using Cafet_Backend.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Cafet_Backend.Repository;

public class CartRepository : ICartRepository
{

    private readonly CafeContext CafeContext;
    private readonly ILogger<CartRepository> Logger;
    public CartRepository(CafeContext cafeContext, ILogger<CartRepository> logger)
    {
        CafeContext = cafeContext;
        this.Logger = logger;
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
        Cart? userCart = await GetCart(cartId);

        if (userCart == null)
            return false;
        
        userCart.FoodCartData.Clear();
        await SaveAsync();
        return true;
    }

    public async Task SaveAsync()
    {
        await CafeContext.SaveChangesAsync();
    }

    public async Task<UserCartData?> AddOrUpdateCart(User user, CartAddition cartAddition)
    {
        //Get the cart of the user
        Cart cartOfUser = await GetOrCreate(user);

        //Checking if the user has already added this food in his cart...
        UserCartData? firstOrDefault = cartOfUser.FoodCartData.FirstOrDefault(f => f.FoodId == cartAddition.FoodId);
        //If this is the first time the user is adding it into his cart or updating the cart with new quantity?
        
        //If no previous record for cart with this id exists!
        if (firstOrDefault == null)
        {
            //Checks if there are enough stock for the food
            DailyStock? inStock = await GetFoodWithQuantity(cartAddition.FoodId, cartAddition.Quantity);
            
            //If not enough stock, The cart is rejected
            if (inStock == null)
                return null;

            //Else new cart item is created and added
            UserCartData itemData = new UserCartData()
            {
                FoodId = inStock.Id,
                FoodName = inStock.Food.Name,
                Quantity = cartAddition.Quantity
            };
            
            cartOfUser.FoodCartData.Add(itemData);
            await CafeContext.SaveChangesAsync();
            return itemData;
        }
        //If the user is updating the cart
        else
        {
            //Since the quantity sent by the client is the new amount of items required
            int newQuantity = cartAddition.Quantity;
            //Checks whether they are in stock!
            DailyStock? inStock = await GetFoodWithQuantity(cartAddition.FoodId, cartAddition.Quantity);
            if (inStock == null)
            {
                //If the updated quantity is null, Will just return as the item with updated quantity is not in stock anymore
                return null;
            }
            else
            {
                firstOrDefault.Quantity = cartAddition.Quantity;
                await CafeContext.SaveChangesAsync();
                return firstOrDefault;
            }
            
        }
    }

    public async Task RemoveItemFromUserCart(User user, int foodId)
    {
        //Get the cart of the user
        Cart cartOfUser = await GetOrCreate(user);

        UserCartData? data = cartOfUser.FoodCartData.FirstOrDefault(cd => cd.FoodId == foodId);
        if(data == null)
            return;

        cartOfUser.FoodCartData.Remove(data);
        await CafeContext.SaveChangesAsync();
        return;
    }

    private async Task<Cart> GetOrCreate(User user)
    {
        //Get the cart of the user
        Cart? cartOfUser = await CafeContext.Carts.FirstOrDefaultAsync(c => c.CartId == user.CartId);
        //Check if user has a cart and if not create a new cart and attach it to the user.
        //A save db has been called here to update the new changes and get the new id of the cart
        if (cartOfUser == null)
        {
            Logger.LogWarning("Failed to locate the cart of user "+user.EmailAddress+".");
            await DeleteCart(user.CartId);

            Cart cart = new Cart()
            {
                FoodCartData = new List<UserCartData>(),
                LastUpdated = DateTime.Now,
            };

            
            user.CartId = cart.CartId;
            cartOfUser = cart;
            await CafeContext.SaveChangesAsync();
            Logger.LogWarning("Loaded new cart for user "+user.EmailAddress+" with id "+cart.CartId);
        }

        return cartOfUser;
    }




    public async Task<List<CartDto>> GetProcessedCart(Guid cartId)
    {
        List<CartDto> cartDtos = new List<CartDto>();

        Cart? cart = await GetCart(cartId);

        if (cart == null)
            return cartDtos;
        
        
        return cartDtos;
    }

    private async Task<DailyStock?> GetFoodWithQuantity(int foodId, int quantity)
    {
        return await CafeContext.Stocks
            .Include(s => s.Food)
            .FirstOrDefaultAsync(s => s.FoodId == foodId && quantity <= s.CurrentStock);
    }

    public async Task<Cart?> GetCart(Guid cartId)
    {
        return await CafeContext.Carts.FirstOrDefaultAsync(cart => cart.CartId == cartId);
    }
}