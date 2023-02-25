using Cafet_Backend.Models;

namespace Cafet_Backend.Specification;

public class OrderHistorySpecification : Specification<Order>
{
    public OrderHistorySpecification(OrderHistorySpecificationParam param)
    {
        AddInclude(x => x.OrderItems);
        AddInclude(x => x.OrderPlacedFor);
        
        if (param.OnlyActive)
        {
            AddFilterCondition(x => x.OrderDelivered == null);
        }

        if (!string.IsNullOrEmpty(param.From))
        {
            try
            {
                DateTime fromDate = DateTime.Parse(param.From);
                AddFilterCondition(x => x.OrderPlaced.Date >= fromDate.Date);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.WriteLine("Invalid date format is provided for From parameter");
            }
        }

        if (!string.IsNullOrEmpty(param.To))
        {
            try
            {
                DateTime toDate = DateTime.Parse(param.To);
                AddFilterCondition(x => x.OrderPlaced.Date <= toDate.Date);
            }
            catch (Exception e)
            {
                Console.WriteLine("Invalid date format is provided for To parameter");
            }
        }
    }
}

public class OrderHistorySpecificationParam
{
    public string? From { get; set; }
    
    public string? To { get; set; }
    
    public bool OnlyActive { get; set; }
    
    public int? PaymentStatus { get; set; }
    
}