using System.Collections.Immutable;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Cafet_Backend.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Cafet_Backend.Provider;

public class TokenService
{


    
    public readonly SymmetricSecurityKey SecurityKey;
    public readonly IOptions<JwtConfig> JwtConfig;

    public TokenService(IOptions<JwtConfig> jwtConfig)
    {
        JwtConfig = jwtConfig;
        SecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtConfig.Value.Token));
    }

    public string CreateToken(List<Claim> claimList)
    {
        SigningCredentials signingCredentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha512Signature);
        SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(claimList),
            Expires = DateTime.Now.AddHours(JwtConfig.Value.AccessTokenLifeTimeHours),
            Issuer = JwtConfig.Value.Issuer,
            SigningCredentials = signingCredentials
        };

        
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken securityToken = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(securityToken);
    }
    
    public string CreateRefreshToken(List<Claim> claimList)
    {
        SigningCredentials signingCredentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha512Signature);
        SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(claimList),
            Expires = DateTime.Now.AddDays(JwtConfig.Value.RefreshTokenLifeTimeDays),
            SigningCredentials = signingCredentials
        };

        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken securityToken = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(securityToken);
    }

    public IEnumerable<Claim> GetClaimsIfValid(string? token)
    {
        if(string.IsNullOrEmpty(token))
            return ImmutableList<Claim>.Empty;
        
        TokenValidationParameters validationParameters = new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtConfig.Value.Token))
        };
        
        var tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken validatedToken = null;
        try
        {
            tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
        }
        catch (Exception ignored)
        {
            return ImmutableList<Claim>.Empty;
        }
        
        if(validatedToken == null)
            return ImmutableList<Claim>.Empty;
        
        var jwtToken = (JwtSecurityToken)validatedToken;
        return jwtToken.Claims.ToImmutableList();
    }
}