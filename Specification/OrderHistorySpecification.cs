﻿using Cafet_Backend.Models;

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
                Console.WriteLine(param.From);
                DateTime fromDate = DateTime.Parse(param.From);
                Console.WriteLine(fromDate.Date);
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
                Console.WriteLine(param.To);
                DateTime toDate = DateTime.Parse(param.To);
                Console.WriteLine(toDate.Date);
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
}