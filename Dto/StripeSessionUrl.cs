namespace Cafet_Backend.Dto;

public class StripeSessionUrl
{
    public string Url { get; set; }

    public StripeSessionUrl(string url)
    {
        Url = url;
    }
}