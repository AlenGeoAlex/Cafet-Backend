using System.ComponentModel.DataAnnotations.Schema;

namespace Cafet_Backend.Models;

public class WalletHistory : KeyedEntity<int>
{
    [ForeignKey("RecipientId")]
    public User Recipient { get; set; }
    
    public int RecipientId { get; set; }
    
    [ForeignKey("SenderId")]
    public User? Sender { get; set; }
    
    public int? SenderId { get; set; }
    
    [Column(TypeName = "datetime2")]
    public DateTime RechargeTime { get; set; }
    
    public bool RechargeStatus { get; set;}
    
    public string? FailReason { get; set; }
    
    [Column(TypeName = "smallmoney")]
    public double Amount { get; set; }

    [NotMapped]
    public bool IsSelfRecharge
    {
        get => SenderId == RecipientId;
        private set { throw new NotImplementedException(); }
    }
    
    public bool Credit { get; set; }
    
    [NotMapped]
    public bool Debit
    {
        get => !Credit;
    }
}