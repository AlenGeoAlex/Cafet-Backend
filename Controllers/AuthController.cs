using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Cafet_Backend.Abstracts;
using Cafet_Backend.Dto;
using Cafet_Backend.Dto.InputDtos;
using Cafet_Backend.Interfaces;
using Cafet_Backend.Models;
using Cafet_Backend.Provider;
using Cafet_Backend.QueryParams;
using JwtUtils.Extensions;
using Microsoft.AspNetCore.Authorization;
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
    private readonly AbstractRefreshTokenManager RefreshTokenManager;
    public AuthController(IUserRepository userRepository, IRoleRepository roleRepository, TokenService tokenService, IMapper mapper, AbstractRefreshTokenManager tokenManager)
    {
        _userRepository = userRepository;
        this.TokenService = tokenService;
        this.roleRepository = roleRepository;
        this.RefreshTokenManager = tokenManager;
        this.mapper = mapper;
    }

    [HttpPost("login/")]
    [ProducesResponseType(typeof(BadRequestObjectResult), 400)]
    [ProducesResponseType(typeof(UnauthorizedObjectResult), 401)]
    [ProducesResponseType(typeof(CredentialsDto), 200)]
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
            new Claim("role", userOfEmail.Role.RoleName),
        };

        string userToken = TokenService.CreateToken(claims);
        string refreshToken = await RefreshTokenManager.GenerateAndStoreRefreshToken(userOfEmail.Id);
        
        CredentialsDto credentialsDto = mapper.Map<User, CredentialsDto>(userOfEmail);
        credentialsDto.AccessToken = userToken;
        credentialsDto.RefreshToken = refreshToken;

        return Ok(credentialsDto);
    }

    [HttpPost("register/")]
    [ProducesResponseType(typeof(BadRequestObjectResult), 400)]
    [ProducesResponseType(typeof(CredentialsDto), 201)]
    public async Task<IActionResult> Register([FromBody] RegistrationParam param)
    {
        
        bool exists = await _userRepository.Exists(param.EmailAddress);
        if (exists)
        {
            return BadRequest("This user name is already taken");
        }
        
        User? user = await _userRepository.TryRegister(param);
        if(user == null)
        {
            return BadRequest("Failed to generate the user");
        }

        if (string.IsNullOrEmpty(param.Password))
        {
            return Ok();
        }
        User? userOfEmail = await _userRepository.GetUserOfEmail(user.EmailAddress);
        if (userOfEmail == null)
        {
            return BadRequest("Failed to register the user");
        }
        
        List<Claim> claims = new List<Claim>()
        {
            new Claim(JwtRegisteredClaimNames.NameId, userOfEmail.Id.ToString()),
            new Claim("role", userOfEmail.Role.RoleName),

        };

        string userToken = TokenService.CreateToken(claims);
        string refreshToken = await RefreshTokenManager.GenerateAndStoreRefreshToken(userOfEmail.Id);

        CredentialsDto credentialsDto = mapper.Map<User, CredentialsDto>(userOfEmail);
        credentialsDto.AccessToken = userToken;
        credentialsDto.RefreshToken = refreshToken;

        return Created("/user/"+userOfEmail.Id,JsonConvert.SerializeObject(credentialsDto));
    }

    [HttpPost("reset-pass")]
    public async Task<ActionResult> ResetUserPassword([FromBody] ResetPassword resetPassword)
    {
        User? userOfEmail = await _userRepository.GetUserOfEmail(resetPassword.EmailAddress);

        if (userOfEmail == null)
            return BadRequest("No email address is found!");

        User? changedUser = await _userRepository.ResetPassword(userOfEmail);

        if (changedUser == null)
            return BadRequest("Failed to reset the password");
        
        return Ok();
    }
}