using Cafet_Backend.Dto;
using Cafet_Backend.Models;
using Cafet_Backend.Specification;

namespace Cafet_Backend.Interfaces;

public interface IOrderRepository
{
    Task<Order?> CreateOrder(ProcessedOrder processedOrder, User orderPlacedBy, User orderPlacedFor);

    Task<Order?> CreateOrderForPayment(ProcessedOrder processedOrder, User orderPlacedBy,
        User orderPlacedFor);

    void DeleteOrderDataOfAsync(int userId);

    Task<List<Order>> GetOrdersFor(ISpecification<Order> param);

    Task<string?> MarkOrderAsComplete(Guid orderId);

    Task<string?> MarkOrderAsFailed(Guid orderId, string reason = null);

    Task<bool> MakeupFailedOrderStock(Guid orderId);

    Task<Order?> GetOrderOfId(Guid orderId);

    Task SaveAsync();

    Task CancelPendingOrders();
}