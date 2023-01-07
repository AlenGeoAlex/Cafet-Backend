using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cafet_Backend.Models;

public class Cart
{
    [Key]
    public Guid CartId { get; set; }
    
    public Dictionary<int, int> CartData { get; set; }
    
    [Column(TypeName = "Datetime2")]
    public DateTime LastUpdated { get; set; }
}