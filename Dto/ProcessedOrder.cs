using Cafet_Backend.Dto.InputDtos;
using Stripe.Checkout;

namespace Cafet_Backend.Dto;

public class ProcessedOrder
{
    public Guid OrderId { get; set; }
    
    public bool OrderSuccessful { get; set; }
    
    public Dictionary<int, bool> OrderStatus { get; set; }
    
    public Dictionary<int, int> OrderFoodQuantity { get; set; }

    public double OrderCost { get; set; }

    public ProcessedOrder(FoodOrder[] foodOrders)
    {
        OrderSuccessful = false;
        OrderStatus = new Dictionary<int, bool>();
        OrderFoodQuantity = new Dictionary<int, int>();
        foreach (FoodOrder order in foodOrders)
        {
            OrderStatus.Add(order.FoodId, false);
            OrderFoodQuantity.Add(order.FoodId, order.OrderQuantity);
        }
    }

    public ProcessedOrder(List<FoodOrder> foodOrders)
    {
        OrderSuccessful = false;
        OrderStatus = new Dictionary<int, bool>();
        OrderFoodQuantity = new Dictionary<int, int>();
        foreach (FoodOrder order in foodOrders)
        {
            OrderStatus.Add(order.FoodId, false);
            OrderFoodQuantity.Add(order.FoodId, order.OrderQuantity);
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