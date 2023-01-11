using Cafet_Backend.Interfaces;
using Cafet_Backend.Manager;

namespace Cafet_Backend.Helper;

public class MailService : IMailService
{
    public Task SendMailAsync(MailModel model, string[] param)
    {
        return Task.CompletedTask;
        
    }
}