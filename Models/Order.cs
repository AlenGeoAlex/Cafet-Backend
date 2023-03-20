using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cafet_Backend.Models;

public class Order : KeyedEntity<Guid>
{
     
     [Column(TypeName = "datetime2")]
     public DateTime OrderPlaced { get; set; }
     
     [Column(TypeName = "datetime2")]
     public DateTime? OrderDelivered { get; set; }
     
     
     [Column(TypeName = "datetime2")]
     public DateTime? OrderCancelled { get; set; }
     
     [ForeignKey("OrderPlacedForId")]
     public User OrderPlacedFor { get; set; }
     
     public int OrderPlacedForId { get; set; }
     
     [ForeignKey("OrderPlacedById")]
     public User? OrderPlacedBy { get; set; }
     
     public int? OrderPlacedById { get; set; }
     
     [Required]
     public List<OrderItems> OrderItems { get; set; }
     
     [Column(TypeName = "smallmoney")]
     public double OrderAmount { get; set; }

     [NotMapped]
     public int TotalQuantity
     {
          get => OrderItems.Count;
     }
     
     [Required]
     public bool WalletPayment { get; set; }
     
     [Required]
     public bool Cancelled { get; set; }
     
     [Required]
     public PaymentStatus PaymentStatus { get; set; }
     
     [Column(TypeName = "datetime2")]
     public DateTime? PaymentStatusUpdatedAt { get; set; }
     
     public string? PaymentFailedReason { get; set; }

     [NotMapped]
     public bool IsFinished
     {
          get => OrderCancelled != null || OrderDelivered != null;
     }
}

public enum PaymentStatus
{
     Pending,
     Success,
     Cancelled
}