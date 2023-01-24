using Cafet_Backend.Abstracts;

namespace Cafet_Backend.Provider.Image;

public class FoodImageProvider : AbstractImageProvider
{
    public FoodImageProvider(IWebHostEnvironment environment, ILogger<AbstractImageProvider> logger) : base("_food", environment, logger)
    {
    }
}