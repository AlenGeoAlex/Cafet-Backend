using System.ComponentModel.DataAnnotations.Schema;

namespace Cafet_Backend.Models;

public class DailyStock : KeyedEntity<int>
{
    [ForeignKey("FoodId")] public Food Food { get; set; }
    
    public int FoodId { get; set; }
    
    public long FoodStock { get; set; }
    
    public long CurrentStock { get; set; }
    
}