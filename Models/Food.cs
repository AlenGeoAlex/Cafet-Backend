using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cafet_Backend.Models;

public class Food : KeyedEntity
{
    [MaxLength(30)] public string Name { get; set; }

    public string FoodDescription { get; set; }

    public string FoodImage { get; set; }

    [Column(TypeName = "Smallmoney")]public double FoodPrice { get; set; }

    public int CategoryId { get; set; }

    [ForeignKey("CategoryId")]
    public FoodCategory Category { get; set; }
}