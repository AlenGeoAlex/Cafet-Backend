namespace Cafet_Backend.Dto;

public class FoodDto
{
    public int FoodId { get; set; }
    
     public string Name { get; set; }

    public string FoodDescription { get; set; }

    public string FoodImage { get; set; }

    public double FoodPrice { get; set; }

    public int CategoryId { get; set; }
    
    public string Category { get; set; }
}