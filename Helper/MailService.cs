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
    public MailService(IOptions<MailConfiguration> mailConfiguration)
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
        Console.WriteLine("Send Mail");
        if (model == null)
        {
            Console.WriteLine("The model provided is null");
            return false;
        }
        string body;
        try
        {
            body = string.Format(model.Body, param);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return false;
        }

        MailMessage mailMessage = new MailMessage("no-reply@cafet.com", emailAddress, model.Subject, body);
        mailMessage.IsBodyHtml = true;
        
        try
        {
            Console.WriteLine(mailConfiguration.Credentials.Hostname);
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
                cleSmtpClient.SendAsync(mailMessage, null);
                Console.WriteLine(123);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return false;
        }
        
        return true;

    }
}