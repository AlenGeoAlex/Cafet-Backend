using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cafet_Backend.Models;

public class UserCartData : KeyedEntity<int>
{
    public int FoodId { get; set; }
    
    [ForeignKey("FoodId")]
    public Food Food { get; set; }
    
    [JsonIgnore]
    public virtual Cart Cart { get; set; }
    
    public Guid CartId { get; set; }
    public string FoodName { get; set; }
    
    public int Quantity { get; set; }
    
    [Column(TypeName = "datetime2")]
    public DateTime LastUpdated { get; set; }
}