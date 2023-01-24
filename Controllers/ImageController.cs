using Cafet_Backend.Abstracts;
using Cafet_Backend.Manager;
using Microsoft.AspNetCore.Mvc;

namespace Cafet_Backend.Controllers;

public class ImageController : AbstractController
{

    private readonly ImageProviderManager ImageProviderManager;
    private readonly IWebHostEnvironment Environment;
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
    
    [HttpGet("upload")]
    public async Task<IActionResult> Upload()
    {
        List<string> fileNames = await CopyFilesFromRequest(Environment, Request);
        return Ok();
    }

    public static async Task<List<string>> CopyFilesFromRequest(IWebHostEnvironment environment, HttpRequest request)
    {
        List<string> fileNames = new List<string>();
        string webRoot = $"{environment.WebRootPath}" +
                         $"{Path.PathSeparator}" +
                         $"images" +
                         $"{Path.PathSeparator}";

        IFormCollection formData = await request.ReadFormAsync();
        
        foreach (IFormFile eachFile in formData.Files)
        {
            string fileName = $"{webRoot}{eachFile.Name}";

            try
            {
                if(System.IO.File.Exists(fileName))
                    System.IO.File.Delete(fileName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            try
            {
                using (FileStream stream = new FileStream(fileName, FileMode.Create))
                {
                    await eachFile.CopyToAsync(stream);
                    fileNames.Add(fileName);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        return fileNames;
    }
}