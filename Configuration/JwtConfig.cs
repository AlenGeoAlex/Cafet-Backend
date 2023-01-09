namespace Cafet_Backend.Configuration;

public class JwtConfig : AbstractConfigurationOptions
{
    
    public string Token { get; set; }
    
    public int AccessTokenLifeTimeHours { get; set; }
    
    public int RefreshTokenLifeTimeDays { get; set; }
    
    public override string ConfigBinder()
    {
        return "JwtConfig";
    }
}