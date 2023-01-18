using System.ComponentModel.DataAnnotations;

namespace Cafet_Backend.Dto.InputDtos;

public class StaffFoodOrder
{
    [Required]
    public bool PaymentMethod { get; set; }
    
    [Required]
    public EmailQueryDto User{ get; set; }
    
    [Required] public List<FoodOrder> SelectedFood { get; set; }
    
}

