namespace Cafet_Backend.Dto;

public class StaffCheckOrderDto
{
    public string OrderId { get; set; }
    public string OrderedEmail { get; set; }
    public string OrderedDate { get; set; }
    public string OrderedTime { get; set; }
    public string OrderedUserName { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsCancelled { get; set; }
    public List<StaffCheckFoodDto> OrderedFoods { get; set; }
    public int PaymentStatus { get; set; }
    
    public bool PaymentMethod { get; set; }
    public string PaymentStatusRaw { get; set; }
    public string? PaymentStatusUpdatedAt { get; set; }
    public string? PaymentFailStatusReason { get; set; }
}

public class StaffCheckFoodDto
{
    public int FoodId { get; set; }
    public string FoodName { get; set; }
    public string FoodCategory { get; set; }
    public bool FoodType { get; set; }
    public double FoodPrice { get; set; }
    public int FoodQuantity { get; set; }
    public string FoodImageUrl { get; set; }
}