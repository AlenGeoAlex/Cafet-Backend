namespace Cafet_Backend.Configuration;

public class StripeConfiguration : AbstractConfigurationOptions
{
    public string SecretToken { get; set; }
    
    public Redirections Redirections { get; set; }

    public Redirections WalletRedirection { get; set; }
    public StripeConfiguration()
    {
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

    public Redirections()
    {
    }
}