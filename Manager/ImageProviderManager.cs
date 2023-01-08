using Cafet_Backend.Provider.Image;

namespace Cafet_Backend.Manager;

public class ImageProviderManager
{
    public readonly FoodImageProvider FoodImageProvider;
    public readonly IWebHostEnvironment? WebHostEnvironment;
    public ImageProviderManager(IWebHostEnvironment webHostEnvironment)
    {
        this.WebHostEnvironment = WebHostEnvironment;
        this.FoodImageProvider = new FoodImageProvider(webHostEnvironment);
    }
}