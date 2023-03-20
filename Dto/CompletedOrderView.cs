namespace Cafet_Backend.Dto;

public class CompletedOrderView : StaffCheckOrderDto
{

    public bool IsFinished { get; set; }
    
    public string? OrderDeliveredAt { get; set; }
    
    public string? OrderCancelledAt { get; set; }
}