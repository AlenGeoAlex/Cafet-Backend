using System.Net;
using System.Net.Mail;
using Cafet_Backend.Configuration;
using Cafet_Backend.Interfaces;
using Cafet_Backend.Manager;
using Microsoft.Extensions.Options;

namespace Cafet_Backend.Helper;

public class MailService : IMailService
{
    
    private readonly NetworkCredential NetworkCredential;
    private readonly MailConfiguration mailConfiguration;
    private readonly ILogger<MailService> Logger;
    public MailService(IOptions<MailConfiguration> mailConfiguration, ILogger<MailService> logger)
    {
        this.NetworkCredential = new NetworkCredential()
        {
            UserName = mailConfiguration.Value.Credentials.Username,
            Password = mailConfiguration.Value.Credentials.Password,
        };
        this.mailConfiguration = mailConfiguration.Value;
    }
    
    public async Task<bool> SendMailAsync(MailModel? model, string emailAddress ,string[] param)
    {
        if (model == null)
        {
            Logger.LogWarning("The provided MailModel is empty! Aborting mail task...");
            return false;
        }
        string body;
        try
        {
            body = string.Format(model.Body, param);
        }
        catch (Exception e)
        {
            Logger.LogError("Failed to parse the mail body", e);
            return false;
        }

        MailMessage mailMessage = new MailMessage("no-reply@cafet.com", emailAddress, model.Subject, body);
        mailMessage.IsBodyHtml = true;
        
        try
        {
            SmtpClient cleSmtpClient = null;
            using (
                cleSmtpClient = new SmtpClient(mailConfiguration.Credentials.Hostname, mailConfiguration.Credentials.Port)
                {
                    UseDefaultCredentials = false,
                    Credentials = NetworkCredential,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network
                }
            )
            {
                await cleSmtpClient.SendMailAsync(mailMessage);
                cleSmtpClient.SendCompleted += (sender, args) =>
                {
                    Logger.LogInformation("Mail Status: ");
                    Logger.LogInformation($"Recipient: {emailAddress}");
                    Logger.LogInformation($"Subject: {model.Subject}");
                    Logger.LogInformation($"Cancelled: {args.Cancelled}");
                    if (args.Error != null)
                    {
                        Logger.LogError("Error Occured ",args.Error);
                    }
                };
            }
        }
        catch (Exception e)
        {
            Logger.LogError("An unknown error occured while sending a mail.", e);
            return false;
        }
        
        return true;

    }
}