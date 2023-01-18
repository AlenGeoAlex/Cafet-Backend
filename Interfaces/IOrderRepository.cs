using Cafet_Backend.Dto;
using Cafet_Backend.Models;

namespace Cafet_Backend.Interfaces;

public interface IOrderRepository
{
    Task<Order?> CreateOrder(ProcessedOrder processedOrder, User orderPlacedBy, User orderPlacedFor);

    void DeleteOrderDataOfAsync(int userId);
}