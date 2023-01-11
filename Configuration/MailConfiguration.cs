namespace Cafet_Backend.Configuration;

public class Mailer
{
    public string FileName { get; set; }
    public string Subject { get; set; }
}

public class MailConfiguration : AbstractConfigurationOptions
{
    public Mailer PasswordReset { get; set; }
    public override string ConfigBinder()
    {
        return "MailerConfiguration";
    }
}

