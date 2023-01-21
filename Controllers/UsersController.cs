using AutoMapper;
using Cafet_Backend.Abstracts;
using Cafet_Backend.Dto;
using Cafet_Backend.Dto.InputDtos;
using Cafet_Backend.Interfaces;
using Cafet_Backend.Manager;
using Cafet_Backend.Models;
using Cafet_Backend.QueryParams;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cafet_Backend.Controllers;

public class UsersController : AbstractController
{
    private readonly IUserRepository UserRepository;
    private readonly IWalletRepository WalletRepository;
    private readonly IMapper Mapper;

    public UsersController(IUserRepository userRepository, IMapper mapper, IWalletRepository walletRepository, MailModelManager mailModelManager)
    {
        UserRepository = userRepository;
        WalletRepository = walletRepository;
        Mapper = mapper;
    }

    [HttpPost("disable")]
    public async Task<IActionResult> DisableAccount([FromBody] AccountStatusParam accountStatusParam)
    {
        User? userOfId = await UserRepository.GetUserOfId(accountStatusParam.AccountId);
        if (userOfId == null)
            return NoContent();

        if (!userOfId.Activated)
            return BadRequest();

        userOfId.Activated = false;
        await UserRepository.SaveAsync();
        return Ok(true);
    }

    [HttpPost("enable")]
    public async Task<IActionResult> EnableAccount([FromBody] AccountStatusParam accountStatusParam)
    {
        User? userOfId = await UserRepository.GetUserOfId(accountStatusParam.AccountId);
        if (userOfId == null)
            return NoContent();

        if (userOfId.Activated)
            return BadRequest();

        userOfId.Activated = true;
        await UserRepository.SaveAsync();
        return Ok(true);
    }
    
    
    [HttpPost("delete")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteAccount([FromBody] AccountStatusParam accountStatusParam)
    {
        User? userOfId = await UserRepository.GetUserOfId(accountStatusParam.AccountId);
        if (userOfId == null)
            return NoContent();

        if (userOfId.Deleted)
            return BadRequest();

        userOfId.Deleted = true;
        await UserRepository.SaveAsync();
        return Ok(true);
    }
    
    [HttpGet]
    public async Task<IReadOnlyList<UserDto>> GetAllUsers()
    {
        List<User> allUser = await UserRepository.GetActiveUsers();

        List<UserDto> dtoList = new List<UserDto>();
        foreach (var user in allUser)
        {
            dtoList.Add(Mapper.Map<User, UserDto>(user));
        }
        return dtoList;
    }

    [HttpGet("me")]
    public async Task<ActionResult> GetMyself()
    {
        User? requestAuthor = Request.HttpContext.Items["User"] as User;
        
        if(requestAuthor == null)
            return Forbid();

        return Ok(Mapper.Map<UserDto>(requestAuthor));
    }

    [HttpGet("email/{address}")]
    public async Task<ActionResult<UserDto>> GetUserOfEmailAddress(string address)
    {
        User? userOfEmail = await UserRepository.GetUserOfEmail(address);

        if (userOfEmail == null)
            return NoContent();

        return Ok(Mapper.Map<UserDto>(userOfEmail));
    }

    [HttpPost("wallet-recharge")]
    [Authorize(Roles = "Staff, Customer")]
    public async Task<ActionResult> WalletRecharge([FromBody] WalletRecharge inputParams)
    {
        User? requestAuthor = Request.HttpContext.Items["User"] as User;
        
        if(requestAuthor == null)
            return Forbid();
        
        if (inputParams.BalanceToAdd <= 0)
            return Ok();

        User? userOfEmail = await UserRepository.GetUserOfEmail(inputParams.EmailAddress);
        if (userOfEmail == null)
            return BadRequest("The user is unknown!");

        bool credit = await WalletRepository.Credit(userOfEmail.Id, requestAuthor.Id, inputParams.BalanceToAdd);

        if (!credit)
            return BadRequest("Failed to update credit!");

        return Ok();
    }

    [HttpPost("update")]
    public async Task<ActionResult<UserDto>> UpdateProfile()
    {
        User? requestAuthor = Request.HttpContext.Items["User"] as User;
        
        if(requestAuthor == null)
            return Forbid("Failed to locate the author of the request!");

        IFormCollection collection = await Request.ReadFormAsync();
        var EmailAddress = collection["EmailAddress"];

        if (requestAuthor.EmailAddress != EmailAddress)
        {
            if (requestAuthor.Role.RoleName != Role.Administrator.RoleName)
                return Forbid("Your role doesn't permit to edit out users profile information!");
        }

        var FirstName = collection["FirstName"];
        var LastName = collection["LastName"];
        var PhoneNumber = collection["PhoneNumber"];
        var Password = collection["Password"];
        bool shouldChangePassword = Password.Count >= 8;
        bool shouldUpdateProfile = false;
        IFormFile? imageFile = collection.Files["Image"];
        if (imageFile != null)
            shouldUpdateProfile = true;

        ProfileUpdate profileUpdate = new ProfileUpdate()
        {
            EmailAddress = EmailAddress,
            FirstName = FirstName,
            LastName = LastName,
            PhoneNumber = PhoneNumber,
            Password = Password,
            ImageFile = imageFile
        };

        User? changed = await UserRepository.UpdateUser(profileUpdate);

        if (changed == null)
            return BadRequest("Failed to update the user");
        
        return Ok(Mapper.Map<UserDto>(changed));
    }

}