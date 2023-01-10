using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Cafet_Backend.Abstracts;
using Cafet_Backend.Configuration;
using Cafet_Backend.Helper.Cache;
using Microsoft.Extensions.Options;

namespace Cafet_Backend.Provider.RTProvider;

public class InMemoryProvider : AbstractRefreshTokenManager
{

    private readonly CachedDictionary<Guid, int> CachedDictionary;

    public InMemoryProvider(IOptions<JwtConfig> configuration, TokenService tokenService) : base(configuration, tokenService)
    {
        this.CachedDictionary = new CachedDictionary<Guid, int>(configuration.Value.RefreshTokenSettings.CacheTTL);
    }

    public override async Task<string> GenerateAndStoreRefreshToken(int userId)
    {
        
        Guid genGuid = Guid.NewGuid();

        List<Claim> claimsList = new List<Claim>()
        {
            new Claim(JwtRegisteredClaimNames.NameId, genGuid.ToString()),
        };
        
        this.CachedDictionary.Set(genGuid, userId);
        string refreshToken = this.TokenService.CreateRefreshToken(claimsList);
        return refreshToken;
    }
}