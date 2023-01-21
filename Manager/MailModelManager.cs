using Cafet_Backend.Configuration;
using Microsoft.Extensions.Options;

namespace Cafet_Backend.Manager;

public class MailModelManager
{

    public IOptions<MailConfiguration> mailConfigurations { get; set; }
    public IWebHostEnvironment HostEnvironment { get; set; }
    public MailModel? PasswordResetMailModel { get; set; }
    private readonly string baseMailerDirectory;
    public MailModel? PasswordChangeAlert { get; set; }
    
    public MailModel? OrderPlaced { get; set; }
    public MailModelManager(IOptions<MailConfiguration> mailConfigurations, IWebHostEnvironment hostEnvironment)
    {
        this.mailConfigurations = mailConfigurations;
        this.HostEnvironment = hostEnvironment;
        baseMailerDirectory = $"{hostEnvironment.WebRootPath}" +
                             $"{Path.DirectorySeparatorChar}" +
                             $"_mailer" +
                             $"{Path.DirectorySeparatorChar}";
        loadMailModels();
    }

    private void loadMailModels()
    {
        PasswordResetMailModel = loadModel(mailConfigurations.Value.PasswordReset);
        PasswordChangeAlert = loadModel(mailConfigurations.Value.PasswordChangedAlert);
        OrderPlaced = loadModel(mailConfigurations.Value.OrderPlaced);
        if (PasswordResetMailModel == null)
        {
            return;
        }
    }

    private MailModel? loadModel(Mailer mailer)
    {
        string file = AsPathName(mailer.FileName);
        if (!File.Exists(file))
            return null;

        string body = File.ReadAllText(file);
        string subject = mailer.Subject;

        return new MailModel()
        {
            Body = body,
            Subject = subject,
            ToEmail = ""
        };
    }

    public string AsPathName(string fileName)
    {
        return $"{baseMailerDirectory}{fileName}";
    }
    
    
}

public class MailModel
{
    public string ToEmail { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
}