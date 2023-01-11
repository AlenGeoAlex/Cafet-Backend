using AutoMapper;
using Cafet_Backend.Abstracts;
using Cafet_Backend.Dto;
using Cafet_Backend.Interfaces;
using Cafet_Backend.Manager;
using Cafet_Backend.Models;
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

    [HttpGet]
    public async Task<IReadOnlyList<UserDto>> GetAllUsers()
    {
        List<User> allUser = await UserRepository.GetAllUser();

        List<UserDto> dtoList = new List<UserDto>();
        foreach (var user in allUser)
        {
            dtoList.Add(Mapper.Map<User, UserDto>(user));
        }
        return dtoList;
    }
}