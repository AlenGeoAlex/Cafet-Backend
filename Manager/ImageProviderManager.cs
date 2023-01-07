using Cafet_Backend.Provider.Image;

namespace Cafet_Backend.Manager;

public class ImageProviderManager
{
    public readonly FoodImageProvider FoodImageProvider;

    public ImageProviderManager()
    {
        this.FoodImageProvider = new FoodImageProvider();
    }
}