﻿using System.ComponentModel.DataAnnotations.Schema;

namespace Cafet_Backend.Models;

public class UserCartData : KeyedEntity<Guid>
{
    public int FoodId { get; set; }
    
    [ForeignKey("FoodId")]
    public Food Food { get; set; }

    public string FoodName { get; set; }
    
    public int Quantity { get; set; }
    
    
}