using Cafet_Backend.Abstracts;
using Cafet_Backend.Provider.Image;

namespace Cafet_Backend.Manager;

public class ImageProviderManager
{
    public readonly AbstractImageProvider FoodImageProvider;
    public readonly AbstractImageProvider UserImageProvider;
    private readonly IWebHostEnvironment _webHostEnvironment;
    public ImageProviderManager(IWebHostEnvironment webHostEnvironment, ILogger<AbstractImageProvider> logger)
    {
        this._webHostEnvironment = webHostEnvironment;
        this.FoodImageProvider = new FoodImageProvider(webHostEnvironment, logger);
        this.UserImageProvider = new UserImageProvider(webHostEnvironment, logger);
    }
}