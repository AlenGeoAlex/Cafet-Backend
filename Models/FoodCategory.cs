﻿using System.ComponentModel.DataAnnotations;

namespace Cafet_Backend.Models;

public class FoodCategory : KeyedEntity<int>
{
    [MaxLength(30)] public string CategoryName { get; set; }

    public string CategoryDescription { get; set; }
}