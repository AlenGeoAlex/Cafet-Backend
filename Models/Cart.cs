using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cafet_Backend.Models;

public class Cart
{

    public static readonly Cart DummyCart = new Cart()
    {
        CartId = Guid.NewGuid(),
        CartData = new Dictionary<int, int>(),
        LastUpdated = DateTime.Now,
    };

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid CartId { get; set; } = Guid.NewGuid();

    public Dictionary<int, int> CartData { get; set; } = new Dictionary<int, int>();
    
    [Column(TypeName = "Datetime2")]
    public DateTime LastUpdated { get; set; } = DateTime.Now;
}