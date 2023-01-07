namespace Cafet_Backend.Abstracts;

public abstract class AbstractImageProvider
{
    public static readonly string BaseImageDirectory;

    static AbstractImageProvider()
    {
        BaseImageDirectory = $"{Directory.GetCurrentDirectory()}" +
                             $"{Path.DirectorySeparatorChar}" +
                             $"_images" +
                             $"{Path.DirectorySeparatorChar}";
    }

    protected readonly string ProviderName;
    protected readonly string ProviderDirectory;
    
    protected AbstractImageProvider(string providerName)
    {
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
}