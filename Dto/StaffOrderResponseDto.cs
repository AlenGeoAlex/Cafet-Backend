using Cafet_Backend.Dto.InputDtos;

namespace Cafet_Backend.Dto;

public class StaffOrderResponseDto
{
    public Guid OrderId { get; set; }
    
    public bool OrderSuccessful { get; set; }
    
    public Dictionary<int, bool> OrderStatus { get; set; }

    public double OrderCost { get; set; }

    public StaffOrderResponseDto(FoodOrder[] foodOrders)
    {
        OrderSuccessful = false;
        OrderStatus = new Dictionary<int, bool>();
        foreach (FoodOrder order in foodOrders)
        {
            OrderStatus.Add(order.FoodId, false);
        }
    }

    public bool Validate()
    {
        bool status = true;
        foreach (KeyValuePair<int,bool> eachOrderStatus in OrderStatus)
        {
            if (!eachOrderStatus.Value)
            {
                status = false;
                break;
            }
        }

        if (status)
        {
            this.OrderId = Guid.NewGuid();
            this.OrderSuccessful = true;
        }

        return status;
    }

    public void SetAvailableOfFoodId(int foodId)
    {
        this.OrderStatus[foodId] = true;
    }

    public List<int> GetAllFoodIds()
    {
        return this.OrderStatus.Keys.ToList();
    }
}