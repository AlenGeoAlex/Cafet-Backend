using Cafet_Backend.Abstracts;
using Cafet_Backend.Dto;
using Cafet_Backend.Dto.InputDtos;
using Cafet_Backend.Interfaces;
using Cafet_Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cafet_Backend.Controllers;

public class CartController : AbstractController
{
    
    public readonly ILogger<CartController> Logger;
    public readonly ICartRepository CartRepository;
    public readonly IUserRepository UserRepository;

    public CartController(ILogger<CartController> logger, ICartRepository cartRepository, IUserRepository userRepository)
    {
        Logger = logger;
        this.CartRepository = cartRepository;
        this.UserRepository = userRepository;
    }

    [HttpGet]
    public async Task<ActionResult<CartDto>> GetCartOfUser()
    {
        Models.User? requestAuthor = Request.HttpContext.Items["User"] as User;

        if (requestAuthor == null)
            return NoContent();

        CartDto cartDto = await CartRepository.GetProcessedCartOfUser(requestAuthor);

        return Ok(cartDto);
    }
    
    [HttpGet("user/{emailAddress}")]
    [Authorize (Roles = "Admin")]
    public async Task<ActionResult<CartDto>> GetCartOfUser(string emailAddress)
    {
        Models.User? user = await UserRepository.GetUserOfEmail(emailAddress);

        if (user == null)
            return NoContent();
        
        CartDto cartDto = await CartRepository.GetProcessedCartOfUser(user);

        return Ok(cartDto);
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
    
    [HttpGet("test")]
    public async Task<ActionResult> CartTest()
    {
        Models.User? requestAuthor = Request.HttpContext.Items["User"] as User;

        if (requestAuthor == null)
            return Forbid();

        await CartRepository.GetProcessedCartOfUser(requestAuthor);
        return Ok();
    }
}