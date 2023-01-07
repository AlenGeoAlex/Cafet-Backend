namespace Cafet_Backend.Configuration;

public class CorsOrigins : AbstractConfigurationOptions
{

    public List<string> SecuredEndPoints { get; set; } = new List<string>();
    public List<string> OpenEndPoints { get; set; }= new List<string>();


    public override string ConfigBinder()
    {
        return "CorsOrigins";
    }
}