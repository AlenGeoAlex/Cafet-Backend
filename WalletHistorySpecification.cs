using System.Globalization;
using Cafet_Backend.Models;
using Cafet_Backend.Specification;

namespace Cafet_Backend;

public class WalletHistorySpecification : Specification<WalletHistory>
{
    public WalletHistorySpecification(WalletHistorySpecificationParam param)
    {
        if (param.FetchMode.HasValue)
        {
            switch (param.FetchMode)
            {
                case 'd':
                case 'D': //Debit
                    AddFilterCondition(x => !x.Credit);
                    break;
                case 'c':
                case 'C': //Credt
                    AddFilterCondition(x => x.Credit);
                    break;
                default:
                    
                    break;
            }
        }

        if (!string.IsNullOrEmpty(param.From))
        {
            try
            {
                Console.WriteLine(param.From);
                DateTime fromDate = DateTime.Parse(param.From);
                Console.WriteLine(fromDate.Date);
                AddFilterCondition(x => x.RechargeTime.Date >= fromDate.Date);
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
                AddFilterCondition(x => x.RechargeTime.Date <= toDate.Date);
            }
            catch (Exception e)
            {
                Console.WriteLine("Invalid date format is provided for To parameter");
            }
        }
    }
}

public class WalletHistorySpecificationParam
{
    public char? FetchMode { get; set; }
    
    public string? From { get; set; }
    
    public string? To { get; set; }
    
}