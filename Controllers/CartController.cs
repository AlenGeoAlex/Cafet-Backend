using Cafet_Backend.Abstracts;
using Cafet_Backend.Dto.InputDtos;
using Cafet_Backend.Interfaces;
using Cafet_Backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cafet_Backend.Controllers;

public class CartController : AbstractController
{
    
    public readonly ILogger<CartController> Logger;
    public readonly ICartRepository CartRepository;

    public CartController(ILogger<CartController> logger, ICartRepository cartRepository)
    {
        Logger = logger;
        this.CartRepository = cartRepository;
    }

    [HttpPost("add")]
    public async Task<ActionResult> AddToCart([FromBody] CartAddition cartAddition)
    {
        Models.User? requestAuthor = Request.HttpContext.Items["User"] as User;

        if (requestAuthor == null)
            return Forbid();

        if (cartAddition.Quantity <= 0)
            return BadRequest("Quantity must be greater than 0");
        
        UserCartData? data = await CartRepository.AddOrUpdateCart(requestAuthor, cartAddition);
        if (data == null)
            return BadRequest("The item is no more in stock!");

        return Ok(data);
    }

    [HttpDelete("remove/{foodId}")]
    public async Task<ActionResult> RemoveFromCart(int foodId)
    {
        Models.User? requestAuthor = Request.HttpContext.Items["User"] as User;

        if (requestAuthor == null)
            return Forbid();

        await CartRepository.RemoveItemFromUserCart(requestAuthor, foodId);

        return Ok();
    }

    [HttpGet("clear")]
    public async Task<ActionResult> ClearUserCart()
    {
        Models.User? requestAuthor = Request.HttpContext.Items["User"] as User;

        if (requestAuthor == null)
            return Forbid();

        await CartRepository.ClearCart(requestAuthor.CartId);
        return Ok();
    }
}