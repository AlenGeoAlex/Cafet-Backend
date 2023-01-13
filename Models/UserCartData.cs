using System.ComponentModel.DataAnnotations.Schema;

namespace Cafet_Backend.Models;

public class UserCartData : KeyedEntity
{
    public Guid CartId { get; set; }
    
    public int StockId { get; set; }
    
    [ForeignKey("StockId")]
    public DailyStock DailyStock { get; set; }

    public string FoodName { get; set; }
    
    public int Quantity { get; set; }
}