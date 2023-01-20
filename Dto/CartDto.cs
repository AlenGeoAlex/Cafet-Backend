namespace Cafet_Backend.Dto;

public class CartDto
{
    
    public string CartId { get; set; }
    public int Count { get; set; }
    public bool IsValid { get; set; }
    public List<CartDataDto> CartData { get; set; } = new List<CartDataDto>();
    public string LastUpdated { get; set; }

    public bool Validate()
    {
        Count = CartData.Count;
        IsValid = true;
        foreach (CartDataDto dataDto in CartData)
        {
            if (!dataDto.Available)
            {
                IsValid = false;
                return false;
            }
        }

        return true;
    }
}

public class CartDataDto
{
    public int FoodId { get; set; }
    public string FoodName { get; set; }
    public int Quantity { get; set; }
    public bool FoodType { get; set; }
    public string FoodCategory { get; set; }
    public bool Available { get; set; }
    
    public string LastUpdated { get; set; }
}