﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cafet_Backend.Models;

public class Cart : KeyedEntity<Guid>
{

    public static readonly Cart DummyCart = new Cart()
    {
        FoodCartData = new List<UserCartData>(),
        LastUpdated = DateTime.Now,
    };

    public virtual List<UserCartData> FoodCartData { get; set; } = new List<UserCartData>();
    
    [Column(TypeName = "Datetime2")]
    public DateTime LastUpdated { get; set; } = DateTime.Now;
}