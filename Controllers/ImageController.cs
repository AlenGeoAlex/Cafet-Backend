using Cafet_Backend.Abstracts;
using Cafet_Backend.Manager;
using Microsoft.AspNetCore.Mvc;

namespace Cafet_Backend.Controllers;

public class ImageController : AbstractController
{

    private readonly ImageProviderManager ImageProviderManager;

    public ImageController(ImageProviderManager imageProviderManager)
    {
        ImageProviderManager = imageProviderManager;
    }

    [HttpGet("food/{id}")]
    public async Task<IActionResult> GetFoodImage(string id)
    {
        Stream? stream = ImageProviderManager.FoodImageProvider.GetImage(id);
        if (stream == null)
            return NotFound();

        DateTime? lastModified = ImageProviderManager.FoodImageProvider.GetLastModified(id);

        return File(stream, "image/jpeg");
    }
    
}