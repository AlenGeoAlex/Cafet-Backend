using Cafet_Backend.Abstracts;

namespace Cafet_Backend.Provider.Image;

public class FoodImageProvider : AbstractImageProvider
{
    public FoodImageProvider(IWebHostEnvironment environment) : base("_food", environment)
    {
    }
}