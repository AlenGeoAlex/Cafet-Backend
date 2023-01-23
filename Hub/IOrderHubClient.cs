using Cafet_Backend.Dto;
using Cafet_Backend.Models;

namespace Cafet_Backend.Hub;

public interface IOrderHubClient
{
    Task SendOrderUpdate(StaffCheckOrderDto order);

    Task FetchOrderList(List<StaffCheckOrderDto> data);
    
    Task OrderCompleted(string orderId);
    
    Task OrderCancelled(string orderId);

}