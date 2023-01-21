using System.ComponentModel.DataAnnotations;

namespace Cafet_Backend.Dto.InputDtos;

public class StaffFoodOrderRequest : FoodOrderRequest
{

    
    [Required]
    public EmailQueryDto User{ get; set; }
    

    
}

public class FoodOrderRequest
{
    [Required]
    public bool PaymentMethod { get; set; }
    
    [Required] public List<FoodOrder> SelectedFood { get; set; }
}

