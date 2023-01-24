namespace Cafet_Backend.Dto;

public class WalletHistoryDto
{
    public string Date { get; set; }
    public string Time { get; set; }
    
    public double Amount { get; set; }
    public bool Credit { get; set; }
    public string? FailReason { get; set; }
}