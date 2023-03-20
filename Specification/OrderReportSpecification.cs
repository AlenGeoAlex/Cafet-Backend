using Cafet_Backend.Models;

namespace Cafet_Backend.Specification;

public class OrderReportSpecification : Specification<Order>
{
    public OrderReportSpecification(OrderReportSpecificationParam param)
    {
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

        if (param.OnlyCompleted is true)
        {
            AddFilterCondition(order => order.PaymentStatus == PaymentStatus.Success);
        }

        if (param.OnlyCancelled is true)
        {
            AddFilterCondition(order => order.Cancelled);
        }

        if (param.PaymentMethod.HasValue)
        {
            int paymentMethod = param.PaymentMethod.Value;
            switch (paymentMethod)
            {
                case 1:
                    AddFilterCondition(o => o.WalletPayment);
                    break;
                case 2:
                    AddFilterCondition(o => !o.WalletPayment);
                    break;
                default:
                    break;
            }
        }
    }
}

public class OrderReportSpecificationParam
{
    public bool? OnlyCompleted { get; set; }
    
    public bool? OnlyCancelled { get; set; }
    public string? From { get; set; }
    public string? To { get; set; }
    public int? PaymentMethod { get; set; }
}