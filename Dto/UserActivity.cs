namespace Cafet_Backend.Dto;

public class UserActivity
{
    public string ActivityId { get; set; }

    public string ActivityType { get; set; }

    public double Amount { get; set; }
    
    public string ActivityOccurence { get; set; }
    
    public long ActivityOccurenceRaw { get; set; }

    public string? ActivityCompleted { get; set; }
    
    public long? ActivityCompletedRaw { get; set; }
    
    public string? Others { get; set; }
}

public enum ActivityType
{
 CancelledOrder,
 CompletedOrder,
 WalletCredit,
 WalletDebit
}