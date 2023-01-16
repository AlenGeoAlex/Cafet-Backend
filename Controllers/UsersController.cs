using AutoMapper;
using Cafet_Backend.Abstracts;
using Cafet_Backend.Dto;
using Cafet_Backend.Interfaces;
using Cafet_Backend.Manager;
using Cafet_Backend.Models;
using Cafet_Backend.QueryParams;
using Microsoft.AspNetCore.Mvc;

namespace Cafet_Backend.Controllers;

public class UsersController : AbstractController
{


    private readonly IUserRepository UserRepository;
    private readonly IMapper Mapper;

    public UsersController(IUserRepository userRepository, IMapper mapper, MailModelManager mailModelManager)
    {
        UserRepository = userRepository;
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


}