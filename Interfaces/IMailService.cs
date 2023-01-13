using Cafet_Backend.Manager;

namespace Cafet_Backend.Interfaces;

public interface IMailService
{
    Task<bool> SendMailAsync(MailModel? model, string emailAddress, string[] param);
}