using System.Globalization;

namespace Cafet_Backend.Extensions;

public static class HeaderExtension
{
    public static bool IsSingleHeader(this IHeaderDictionary headers, string key) 
    {
        return headers.ContainsKey(key) && headers[key].Count() == 1;
    }
    
    public static string ToTitleCase(this string title)
    {
        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(title.ToLower()); 
    }
}