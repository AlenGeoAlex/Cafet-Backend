namespace Cafet_Backend.Configuration;

public class StripeConfiguration : AbstractConfigurationOptions
{
    public string SecretToken { get; set; }
    
    public Redirections Redirections { get; set; }

    public StripeConfiguration(string secretToken, Redirections redirections)
    {
        SecretToken = secretToken;
        Redirections = redirections;
    }

    public override string ConfigBinder()
    {
        return "Stripe";
    }
}

public class Redirections
{
    public string Success { get; set; }
    
    public string Cancelled { get; set; }

    public Redirections(string success, string cancelled)
    {
        Success = success;
        Cancelled = cancelled;
    }
}