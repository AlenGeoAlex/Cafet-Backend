using AutoMapper;
using Cafet_Backend.Dto;
using Cafet_Backend.Helper;
using Cafet_Backend.Interfaces;
using Cafet_Backend.Models;
using Cafet_Backend.Repository;
using Cafet_Backend.Specification;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
namespace Cafet_Backend.Hub;

public class OrderHub : Hub<IOrderHubClient>
{

    public readonly IMapper Mapper;
    public readonly IOrderRepository OrderRepository;
    public readonly IUserRepository UserRepository;
    public readonly ILogger<OrderHub> Logger;

    public OrderHub(IMapper mapper, IOrderRepository orderRepository, IUserRepository userRepository, ILogger<OrderHub> logger)
    {
        Mapper = mapper;
        OrderRepository = orderRepository;
        UserRepository = userRepository;
        Logger = logger;
    }

    public async System.Threading.Tasks.Task BroadcastOrderUpdate(Order order)
    {
        await Clients.All.SendOrderUpdate(Mapper.Map<StaffCheckOrderDto>(order));
    }

    public async System.Threading.Tasks.Task GetRecentOrders()
    {

        Specification.Specification<Order> specification = new Specification<Order>(
        );

        specification.Limit = 10;
        specification.ApplyOrderByDescending(x => x.OrderPlaced);
        specification.AddFilterCondition(x => x.OrderPlaced.Date == DateTime.Today);
        specification.AddFilterCondition(x => x.OrderCancelled == null);
        specification.AddFilterCondition(x => x.OrderDelivered == null);
        List<Order> list = await OrderRepository.GetOrdersFor(specification);
        await Clients.Client(Context.ConnectionId).FetchOrderList(Mapper.Map<List<StaffCheckOrderDto>>(list));
    }
    
    public async System.Threading.Tasks.Task GetLiveOrders()
    {

        Specification.Specification<Order> specification = new Specification<Order>(
        );

        specification.Limit = 10;
        specification.ApplyOrderByDescending(x => x.OrderPlaced);
        specification.AddFilterCondition(x => x.OrderPlaced.Date == DateTime.Today);
        List<Order> list = await OrderRepository.GetOrdersFor(specification);
        await Clients.Client(Context.ConnectionId).FetchOrderList(Mapper.Map<List<StaffCheckOrderDto>>(list));
    }
}