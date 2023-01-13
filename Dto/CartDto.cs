namespace Cafet_Backend.Dto;

public class CartDto
{
    public string FoodName { get; set; }
    public int FoodId { get; set; }
    public string FoodImageUrl { get; set; }
    public int FoodQuantity { get; set; }
    public bool InStock { get; set; }
    
}