using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cafet_Backend.Models;

public class OrderItems : KeyedEntity<int>
{
    [ForeignKey("OrderId")]
    
    [JsonIgnore]
    public Order Order { get; set; }
    
    public Guid OrderId { get; set; }
    
    public int FoodId { get; set; }
    
    [ForeignKey("FoodId")]
    public Food Food { get; set; }
    
    public string FoodName { get; set; }
    
    public int Quantity { get; set; }
    
    [Column(TypeName = "smallmoney")]
    public double FoodPrice { get; set; }
}