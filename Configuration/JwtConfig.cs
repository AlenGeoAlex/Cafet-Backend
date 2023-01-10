namespace Cafet_Backend.Configuration;

public class JwtConfig : AbstractConfigurationOptions
{
    
    public string Token { get; set; }
    
    public int AccessTokenLifeTimeHours { get; set; }
    
    public int RefreshTokenLifeTimeDays { get; set; }
    
    public string Issuer { get; set; }
    
    public RefreshTokenSettings RefreshTokenSettings { get; set; }
    public override string ConfigBinder()
    {
        return "JwtConfig";
    }
}

public class RefreshTokenSettings
{
    public string CacheMethod { get; set; }
    
    public int CacheTTL { get; set; }
}