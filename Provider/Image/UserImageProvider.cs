using Cafet_Backend.Abstracts;

namespace Cafet_Backend;
//https://www.veryicon.com/icons/internet--web/prejudice/user-128.html
public class UserImageProvider : AbstractImageProvider
{
    public UserImageProvider(IWebHostEnvironment hostEnvironment, ILogger<AbstractImageProvider> logger) : base("_user", hostEnvironment, logger)
    {
    }
}