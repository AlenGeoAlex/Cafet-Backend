namespace Cafet_Backend.Configuration;

public class Mailer
{
    public string FileName { get; set; }
    public string Subject { get; set; }
}

public class MailCredentials
{
    public string Hostname { get; set; }
    public int Port { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}

public class MailConfiguration : AbstractConfigurationOptions
{
    public Mailer PasswordReset { get; set; }
    
    public Mailer PasswordChangedAlert { get; set; }
    public MailCredentials Credentials { get; set; }
    public override string ConfigBinder()
    {
        return "MailerConfiguration";
    }
}

