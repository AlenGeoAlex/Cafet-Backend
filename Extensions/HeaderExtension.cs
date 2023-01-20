namespace Cafet_Backend.Extensions;

public static class HeaderExtension
{
    public static bool IsSingleHeader(this IHeaderDictionary headers, string key) 
    {
        return headers.ContainsKey(key) && headers[key].Count() == 1;
    }
}