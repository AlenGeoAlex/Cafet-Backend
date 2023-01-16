using System.Collections.Immutable;
using System.Text.RegularExpressions;
using AutoMapper;
using Cafet_Backend.Abstracts;
using Cafet_Backend.Dto;
using Cafet_Backend.Dto.InputDtos;
using Cafet_Backend.Interfaces;
using Cafet_Backend.Models;
using Cafet_Backend.QueryParams;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Cafet_Backend.Controllers;

public class OrderController : AbstractController
{

    public readonly IUserRepository UserRepository;
    public readonly IStockRepository StockRepository;
    public readonly IFoodRepository FoodRepository;
    public readonly IMapper Mapper;

    public OrderController(IUserRepository userRepository, IStockRepository stockRepository, IFoodRepository foodRepository, IMapper mapper)
    {
        UserRepository = userRepository;
        StockRepository = stockRepository;
        FoodRepository = foodRepository;
        Mapper = mapper;
    }

    [HttpPost("staff")]
    public async Task<ActionResult> OrderFromStaff([FromBody] StaffFoodOrder inputParam)
    {
        Console.WriteLine(JsonConvert.SerializeObject(inputParam));

        User? userOfEmail = await UserRepository.GetUserOfEmail(inputParam.User.EmailAddress);
        Console.WriteLine(userOfEmail == null);
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

        StaffOrderResponseDto responseDto = new StaffOrderResponseDto(inputParam.SelectedFood);
        List<int> orderedFoodIds = responseDto.GetAllFoodIds();
        List<DailyStock> stockOfFoodIds = await StockRepository.GetStockOfFoodIds(orderedFoodIds);
        List<FoodOrder> foodOrders = new List<FoodOrder>(inputParam.SelectedFood);
        double possibleCost = 0D;

        foreach (DailyStock dailyStock in stockOfFoodIds)
        {
            int eachFoodId = dailyStock.FoodId;
            long inQuantity = dailyStock.CurrentStock;

            FoodOrder? @default = foodOrders.FirstOrDefault(order => order.FoodId == eachFoodId);
            if(@default == null)
                continue;

            int requiredQuantity = @default.OrderQuantity;

            if (requiredQuantity <= inQuantity)
            {
                responseDto.SetAvailableOfFoodId(eachFoodId);
                possibleCost += (dailyStock.Food.FoodPrice * requiredQuantity);
            }
        }


        bool isReady = responseDto.Validate();
        responseDto.OrderCost = possibleCost;
        if (!isReady)
            return Ok(responseDto);

        
        Console.WriteLine(JsonConvert.SerializeObject(responseDto));
        
        
        
        //Pay Using Wallet
        if (inputParam.PaymentMethod)
        {
            
        }

        return Ok();
    }

}