using System.ComponentModel.DataAnnotations;
using Cafet_Backend.Helper;
using Cafet_Backend.Models;

namespace Cafet_Backend.Specification;

public class OrderSearchSpecification : Specification<Order>
{
    
    public OrderSearchSpecificationParam OrderSearchSpecificationParam { get; set; }

    public OrderSearchSpecification(OrderSearchSpecificationParam orderSearchSpecificationParam)
    {
        OrderSearchSpecificationParam = orderSearchSpecificationParam;
        
        Console.WriteLine(orderSearchSpecificationParam.ToString());

        if (orderSearchSpecificationParam.ignoreCancelled.HasValue)
        {
            if( orderSearchSpecificationParam.ignoreCancelled.Value)
                AddFilterCondition(x => x.OrderCancelled != null );
            else
                AddFilterCondition(x => x.OrderCancelled == null);
        }

        if (orderSearchSpecificationParam.ignoreCompleted.HasValue)
        {
            if(orderSearchSpecificationParam.ignoreCompleted.Value)
                AddFilterCondition(x => x.OrderDelivered != null);
            else AddFilterCondition(x => x.OrderDelivered == null);
        }
        
        AddFilterCondition(or => or.Id.ToString().Contains(orderSearchSpecificationParam.SearchParam) || or.OrderPlacedFor.EmailAddress.Contains(orderSearchSpecificationParam.SearchParam));
        
    }
}

public class OrderSearchSpecificationParam
{
    public bool? ignoreCancelled { get; set; }

    public bool? ignoreCompleted { get; set;  }

    [Required]
    public string SearchParam { get; set; } = "";

    public override string ToString()
    {
        return $"IgnoreCancelled : {ignoreCancelled}\n" +
               $"IgnoreCompleted: {ignoreCompleted}\n" +
               $"SearchParam: {SearchParam}";
    }
}