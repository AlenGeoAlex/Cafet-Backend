using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Cafet_Backend.Models;

public class Food : KeyedEntity<int>
{
    [MaxLength(30)] public string Name { get; set; }

    public string FoodDescription { get; set; }

    public string FoodImage { get; set; }

    [Column(TypeName = "Smallmoney")]public double FoodPrice { get; set; }

    public int CategoryId { get; set; }

    [ForeignKey("CategoryId")]
    public FoodCategory Category { get; set; }
    
    public bool Vegetarian { get; set; }
    
    public string Tags { get; set; }

    [NotMapped]
    public List<string> TagCollection
    {
        get => JsonConvert.DeserializeObject<List<string>>(Tags);
        set => Tags = JsonConvert.SerializeObject(value);
    }

    public void AddTag(string tag)
    {
        this.TagCollection.Add(tag);
        this.TagCollection = this.TagCollection;
    }
    
    
    
}