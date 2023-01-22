using Cafet_Backend.Configuration;
using Microsoft.Extensions.Options;

namespace Cafet_Backend.Manager;

public class MailModelManager
{

    public IOptions<MailConfiguration> MailConfigurations { get; set; }
    public IWebHostEnvironment HostEnvironment { get; set; }
    public MailModel? PasswordResetMailModel { get; set; }
    private readonly string BaseMailerDirectory;
    public MailModel? PasswordChangeAlert { get; set; }
    
    public MailModel? OrderPlaced { get; set; }
    
    public MailModel? WalletRecharge { get; set; }
    public MailModelManager(IOptions<MailConfiguration> mailConfigurations, IWebHostEnvironment hostEnvironment)
    {
        this.MailConfigurations = mailConfigurations;
        this.HostEnvironment = hostEnvironment;
        BaseMailerDirectory = $"{hostEnvironment.WebRootPath}" +
                             $"{Path.DirectorySeparatorChar}" +
                             $"_mailer" +
                             $"{Path.DirectorySeparatorChar}";
        LoadMailModels();
    }

    private void LoadMailModels()
    {
        PasswordResetMailModel = LoadModel(MailConfigurations.Value.PasswordReset);
        PasswordChangeAlert = LoadModel(MailConfigurations.Value.PasswordChangedAlert);
        OrderPlaced = LoadModel(MailConfigurations.Value.OrderPlaced);
        WalletRecharge = LoadModel(MailConfigurations.Value.WalletRecharge);
        if (PasswordResetMailModel == null)
        {
            return;
        }
    }

    private MailModel? LoadModel(Mailer mailer)
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
        return $"{BaseMailerDirectory}{fileName}";
    }
    
    
}

public class MailModel
{
    public string ToEmail { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
}