using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Cafet_Backend.Configuration;
using Cafet_Backend.Interfaces;
using Cafet_Backend.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Cafet_Backend.Middleware;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<JwtMiddleware> Logger;
    //private readonly IUserRepository UserRepository;
    private readonly IOptions<JwtConfig> Options;
    public JwtMiddleware(RequestDelegate next, ILogger<JwtMiddleware> logger, IOptions<JwtConfig> jwtConfig)
    {
        _next = next;
        this.Logger = logger;
        this.Options = jwtConfig;
    }

    public async Task Invoke(HttpContext context, IUserRepository userRepository)
    {
        if (context.Request.Headers.ContainsKey("Authorization"))
        {
            string authHeader = context.Request.Headers.Authorization;
            if (!string.IsNullOrEmpty(authHeader))
            {

                string? token = authHeader.Split(" ").LastOrDefault();
                if (!string.IsNullOrEmpty(token))
                {
                    TokenValidationParameters validationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Options.Value.Token))
                    };
                    //Logger.LogInformation(token);
                    var tokenHandler = new JwtSecurityTokenHandler();
                    SecurityToken validatedToken = null;
                    try
                    {
                        tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
                    }
                    catch (Exception ignored)
                    {
                        // ignored
                    }

                    if (validatedToken != null)
                    {
                        var jwtToken = (JwtSecurityToken)validatedToken;
                        Claim? firstOrDefault =
                            jwtToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.NameId);

                        //No Claim
                        if (firstOrDefault == null)
                            return;

                        int userId = Convert.ToInt32(firstOrDefault.Value);
                        User? userOfId = await userRepository.GetUserOfId(userId);

                        if (userOfId == null)
                            return;

                        context.Items["User"] = userOfId;
                    }
                }
            }
        }

        await _next(context);
    }
}