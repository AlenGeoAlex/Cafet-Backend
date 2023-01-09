using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Cafet_Backend.Abstracts;
using Cafet_Backend.Dto;
using Cafet_Backend.Interfaces;
using Cafet_Backend.Models;
using Cafet_Backend.Provider;
using Cafet_Backend.QueryParams;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Newtonsoft.Json;

namespace Cafet_Backend.Controllers;

public class AuthController : AbstractController
{

    private readonly IUserRepository _userRepository;
    private readonly TokenService TokenService;
    private readonly IRoleRepository roleRepository;
    private readonly IMapper mapper;
    public AuthController(IUserRepository userRepository, IRoleRepository roleRepository, TokenService tokenService, IMapper mapper)
    {
        _userRepository = userRepository;
        this.TokenService = tokenService;
        this.roleRepository = roleRepository;
        this.mapper = mapper;
    }

    [HttpPost("login/")]
    [ProducesResponseType(typeof(BadRequestObjectResult), 400)]
    [ProducesResponseType(typeof(UnauthorizedObjectResult), 401)]
    [ProducesResponseType(typeof(UserDto), 200)]
    public async Task<IActionResult> Login([FromBody] LoginParams loginParams)
    {
        string emailAddress = loginParams.EmailAddress;

        if (string.IsNullOrEmpty(emailAddress))
        {
            return BadRequest("No email address provided!");
        }
        
        User? userOfEmail = await _userRepository.GetUserOfEmail(emailAddress);
        if (userOfEmail == null)
        {
            return Unauthorized("Invalid Credentials Provided!");
        }

        HMACSHA512 hasher = new HMACSHA512(userOfEmail.UserSalt);
        byte[] presentComputedHash = hasher.ComputeHash(Encoding.UTF8.GetBytes(loginParams.Password));
        if(presentComputedHash.Length != userOfEmail.PasswordHash.Length)
        {
            return Unauthorized("Invalid Credentials Provided!");
        }
        
        for (var i = 0; i < presentComputedHash.Length; i++)
        {
            if(presentComputedHash[i] != userOfEmail.PasswordHash[i])
            {
                return Unauthorized("Invalid Credentials Provided!");
            }
        }
        
        List<Claim> claims = new List<Claim>()
        {
            new Claim(JwtRegisteredClaimNames.NameId, userOfEmail.Id.ToString()),
        };

        string userToken = TokenService.CreateToken(claims);

        UserDto userDto = mapper.Map<User, UserDto>(userOfEmail);
        userDto.AccessToken = userToken;

        return Ok(userDto);
    }

    [HttpPost("register/")]
    [ProducesResponseType(typeof(BadRequestObjectResult), 400)]
    [ProducesResponseType(typeof(UserDto), 201)]
    public async Task<IActionResult> Register([FromBody] RegistrationParam param)
    {
        
        bool exists = await _userRepository.Exists(param.EmailAddress);
        if (exists)
        {
            return BadRequest("This user name is already taken");
        }

        HMACSHA512 hasher = new HMACSHA512();

        byte[] salt = hasher.Key;
        byte[] passwordHash = hasher.ComputeHash(Encoding.UTF8.GetBytes(param.Password));

        Cart cart = new Cart();
        
        
        Role? roleByName = Role.GetByName(param.Role);
        if (roleByName == null)
        {
            return BadRequest("This user role is unknown");
        }

        Role? roleData = await roleRepository.GetRoleByNameAsync(roleByName.RoleName);
        if (roleData == null)
        {
            return BadRequest("This user role is unknown");
        }
        
        User user = new User()
        {
            EmailAddress = param.EmailAddress,
            FirstName = param.FirstName,
            LastName = param.LastName,
            Cart = cart,
            CartId = cart.CartId,
            RoleId = roleData.Id,
            Activated = true,
            Deleted = false,
            PasswordHash = passwordHash,
            UserSalt = salt,
            ProfileImage = "default.png",
            WalletBalance = 0.0,
        };

        await _userRepository.Register(user);
        User? userOfEmail = await _userRepository.GetUserOfEmail(user.EmailAddress);
        if (userOfEmail == null)
        {
            return BadRequest("Failed to register the user");
        }
        List<Claim> claims = new List<Claim>()
        {
            new Claim(JwtRegisteredClaimNames.NameId, userOfEmail.Id.ToString()),
        };

        string userToken = TokenService.CreateToken(claims);

        UserDto userDto = mapper.Map<User, UserDto>(userOfEmail);
        userDto.AccessToken = userToken;

        return Created("/user/"+userOfEmail.Id,JsonConvert.SerializeObject(userDto));
    }
}