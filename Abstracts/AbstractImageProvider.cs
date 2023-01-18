namespace Cafet_Backend.Abstracts;

public abstract class AbstractImageProvider
{

    protected readonly string ProviderName;
    protected readonly string ProviderDirectory;
    protected readonly string BaseImageDirectory;

    protected AbstractImageProvider(string providerName, IWebHostEnvironment hostEnvironment)
    {
        
        BaseImageDirectory = $"{hostEnvironment.WebRootPath}" +
                             $"{Path.DirectorySeparatorChar}" +
                             $"_images" +
                             $"{Path.DirectorySeparatorChar}";
        
        Console.WriteLine(BaseImageDirectory);
        ProviderName = providerName;
        
        ProviderDirectory = $"{BaseImageDirectory}" +
                            $"{providerName.ToLower()}";
        Console.WriteLine(ProviderDirectory);
        
        InitDirectory();
    }
    private void InitDirectory()
    {
        if (!Directory.Exists(ProviderDirectory))
        {
            Directory.CreateDirectory(ProviderDirectory);
            Console.WriteLine("Created directory "+ProviderDirectory);
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
        File.Delete(fileName);
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