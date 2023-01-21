namespace Cafet_Backend.Dto.InputDtos;

public class FoodOrder
{
    public int FoodId { get; set; }
    
    public string FoodImage { get; set; }
    
    public string FoodName { get; set; }
    
    public double FoodPrice { get; set; }
    
    public int OrderQuantity { get; set; }
    
}