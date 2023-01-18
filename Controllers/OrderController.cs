using System.Collections.Immutable;
using System.Text.RegularExpressions;
using AutoMapper;
using Cafet_Backend.Abstracts;
using Cafet_Backend.Dto;
using Cafet_Backend.Dto.InputDtos;
using Cafet_Backend.Interfaces;
using Cafet_Backend.Models;
using Cafet_Backend.QueryParams;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Cafet_Backend.Controllers;

public class OrderController : AbstractController
{

    public readonly IUserRepository UserRepository;
    public readonly IStockRepository StockRepository;
    public readonly IFoodRepository FoodRepository;
    public readonly IOrderRepository OrderRepository;
    private readonly IWalletRepository WalletRepository;
    public readonly IMapper Mapper;

    public OrderController(IUserRepository userRepository, IStockRepository stockRepository, IFoodRepository foodRepository,IOrderRepository orderRepository, IWalletRepository walletRepository ,IMapper mapper)
    {
        UserRepository = userRepository;
        StockRepository = stockRepository;
        FoodRepository = foodRepository;
        OrderRepository = orderRepository;
        WalletRepository = walletRepository;
        Mapper = mapper;
    }

    [HttpPost("staff")]
    [Authorize(Roles = "Staff")]
    [ProducesResponseType(typeof(ProcessedOrder), 200)]
    public async Task<ActionResult> OrderFromStaff([FromBody] StaffFoodOrder inputParam)
    {
        User? orderedStaffUser = Request.HttpContext.Items["User"] as User;
        if (orderedStaffUser == null)
            return Forbid("Failed to locate the author of the request!");
        
        User? userOfEmail = await UserRepository.GetUserOfEmail(inputParam.User.EmailAddress);
        if (userOfEmail == null)
        {
            bool isEmailValid = Regex.IsMatch(inputParam.User.EmailAddress, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);

            if (!isEmailValid)
            {
                return ValidationProblem("The provided email id is invalid!");
            }

            User? register = await UserRepository.TryRegister(new RegistrationParam()
            {
                EmailAddress = inputParam.User.EmailAddress,
                FirstName = inputParam.User.FirstName,
                LastName = inputParam.User.LastName,
                Password = null,
                Role = "CUSTOMER",
            });

            if (register == null)
            {
                return BadRequest("Failed to create an account for user!");
            }

            userOfEmail = register;
        }

        ProcessedOrder processedOrder = await StockRepository.ProcessOrderResponse(inputParam.SelectedFood);

        if (!processedOrder.OrderSuccessful)
            return BadRequest("Some items are out of stock!");
        
        //Pay Using Wallet
        if (inputParam.PaymentMethod)
        {
            if (userOfEmail.WalletBalance < processedOrder.OrderCost)
            {
                return BadRequest("Insufficient Account Balance");
            }

            bool balanceWithdrawn = await WalletRepository.Withdraw(userOfEmail.Id, orderedStaffUser.Id, processedOrder.OrderCost);
            if(!balanceWithdrawn)
            {
                return BadRequest("Failed to transfer wallet balance!");
            }
        }

        Order? order = await OrderRepository.CreateOrder(processedOrder, orderedStaffUser, userOfEmail);

        if (order == null)
        {
            return BadRequest("Failed to place the order! Try again!");
        }

        processedOrder.OrderId = order.Id;
        
        return Ok(processedOrder);
    }

}