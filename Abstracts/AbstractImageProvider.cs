namespace Cafet_Backend.Abstracts;

public abstract class AbstractImageProvider
{

    protected readonly string ProviderName;
    protected readonly string ProviderDirectory;
    protected readonly string BaseImageDirectory;
    protected readonly ILogger<AbstractImageProvider> Logger;

    protected AbstractImageProvider(string providerName, IWebHostEnvironment hostEnvironment, ILogger<AbstractImageProvider> logger)
    {
        Logger = logger;
        BaseImageDirectory = $"{hostEnvironment.WebRootPath}" +
                             $"{Path.DirectorySeparatorChar}" +
                             $"_images" +
                             $"{Path.DirectorySeparatorChar}";
        
        ProviderName = providerName;
        
        ProviderDirectory = $"{BaseImageDirectory}" +
                            $"{providerName.ToLower()}";
        
        InitDirectory();
    }
    private void InitDirectory()
    {
        if (!Directory.Exists(ProviderDirectory))
        {
            Directory.CreateDirectory(ProviderDirectory);
           Logger.LogInformation("Created directory "+ProviderDirectory);
        }
    }

    public bool Exists(string fileName)
    {
        return File.Exists(AsFileName(fileName));
    }

    public Stream? GetImage(string fileName)
    {
        FileStream imageStream = File.Open(AsFileName(fileName), FileMode.Open);
        return imageStream;
    }

    public string AsFileName(string fileName)
    {
        return $"{ProviderDirectory}{Path.DirectorySeparatorChar}{fileName}";
    }

    public DateTime? GetLastModified(string fileName)
    {
        return File.GetLastWriteTime(AsFileName(fileName));
    }

    public void Delete(string fileName)
    {
        try
        {
            if(Exists(AsFileName(fileName)))
                File.Delete(fileName);
        }
        catch (Exception e)
        {
            Logger.LogError("Failed to delete file "+fileName);
        }
    }

    public string GetDefaultImageName()
    {
        return "default.png";
    }

    public Stream? GetDefaultImage()
    {
        return GetImage(GetDefaultImageName());
    }
}