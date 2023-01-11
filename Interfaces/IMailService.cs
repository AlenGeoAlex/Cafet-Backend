using Cafet_Backend.Manager;

namespace Cafet_Backend.Interfaces;

public interface IMailService
{
    Task SendMailAsync(MailModel? model, string[] param);
}