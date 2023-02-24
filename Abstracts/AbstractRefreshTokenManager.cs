using Cafet_Backend.Configuration;
using Cafet_Backend.Models;
using Cafet_Backend.Provider;
using Microsoft.Extensions.Options;

namespace Cafet_Backend.Abstracts;

public abstract class AbstractRefreshTokenManager
{
    protected readonly IOptions<JwtConfig> Configuration;
    protected readonly TokenService TokenService;

    protected AbstractRefreshTokenManager(IOptions<JwtConfig> configuration, TokenService tokenService)
    {
        Configuration = configuration;
        TokenService = tokenService;
    }

    public abstract Task<string> GenerateAndStoreRefreshToken(int userId);

    public abstract Task<Tuple<string?, int?>> RefreshIfValid(string refreshToken);

    public abstract Task InvalidateToken(string refreshToken);
}