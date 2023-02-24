using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Cafet_Backend.Abstracts;
using Cafet_Backend.Configuration;
using Cafet_Backend.Helper.Cache;
using Microsoft.Extensions.Options;

namespace Cafet_Backend.Provider.RTProvider;

public class InMemoryProvider : AbstractRefreshTokenManager
{

    private readonly CachedDictionary<Guid, int> _cachedDictionary;

    public InMemoryProvider(IOptions<JwtConfig> configuration, TokenService tokenService) : base(configuration, tokenService)
    {
        this._cachedDictionary = new CachedDictionary<Guid, int>(configuration.Value.RefreshTokenSettings.CacheTTL);
    }

    public override async Task<string> GenerateAndStoreRefreshToken(int userId)
    {
        
        Guid genGuid = Guid.NewGuid();

        List<Claim> claimsList = new List<Claim>()
        {
            new Claim(JwtRegisteredClaimNames.NameId, genGuid.ToString()),
        };
        
        this._cachedDictionary.Set(genGuid, userId);
        string refreshToken = this.TokenService.CreateRefreshToken(claimsList);
        return refreshToken;
    }

    public override async Task<Tuple<string?, int?>> RefreshIfValid(string refreshToken)
    {
        IEnumerable<Claim> claimsIfValid = this.TokenService.GetClaimsIfValid(refreshToken);

        Claim? claim = claimsIfValid.FirstOrDefault(s => s.Type == JwtRegisteredClaimNames.NameId);
        
        if(claim == null)
            return new Tuple<string?, int?>(null, null);

        string claimValue = claim.Value;

        bool parsed = Guid.TryParse(claimValue, out Guid parsedId);
        
        if(!parsed)
            return new Tuple<string?, int?>(null, null);

        if (!_cachedDictionary.Has(parsedId))
            return new Tuple<string?, int?>(null, null);

        int i = _cachedDictionary.Get(parsedId);

        _cachedDictionary.Remove(parsedId);
        string token = await GenerateAndStoreRefreshToken(i);

        return new Tuple<string?, int?>(token, i);
    }

    public override async Task InvalidateToken(string refreshToken)
    {
        IEnumerable<Claim> claimsIfValid = this.TokenService.GetClaimsIfValid(refreshToken);

        Claim? claim = claimsIfValid.FirstOrDefault(s => s.Type == JwtRegisteredClaimNames.NameId);

        if (claim == null)
            return;

        string claimValue = claim.Value;

        bool parsed = Guid.TryParse(claimValue, out Guid parsedId);
        
        if(!parsed)
            return;

        if (!_cachedDictionary.Has(parsedId))
            return;

        int i = _cachedDictionary.Get(parsedId);

        _cachedDictionary.Remove(parsedId);
    }
}