using Cafet_Backend.Models;

namespace Cafet_Backend.Specification;

public class UserOrderActivitySpecification : Specification<Order>
{
    public UserOrderActivitySpecification(UserActivitySpecificationParam param)
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

        int id;
        try
        {
             id = Convert.ToInt32(param.User);
        }
        catch (Exception e)
        {
            Console.WriteLine("Failed to parse the user Id");
            throw;
        }
        
        AddFilterCondition(x => x.OrderPlacedFor.Id == id);

    }
}