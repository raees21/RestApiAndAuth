using RestAPI.Common.Enums;
using RestAPI.Models;

namespace RestAPI.Services;

public interface IOrderService
{
    public Task<OrderResponse> CreateOrder(OrderCreate order, Guid userId);
    public Task<IEnumerable<OrderResponse>> GetAllOrders(
        IEnumerable<EOrderStatus> statuses, 
        IEnumerable<EOrderType> types,
        Guid? userId = null);
    public Task<OrderResponse> GetOrder(Guid orderId, Guid userId, EUserType userType);
    public Task UpdateOrderStatus(Guid orderId, EOrderStatus status, Guid userId);
    public Task<IEnumerable<OrderStatusUpdateResponse>> GetAllOrderStatusUpdates(Guid orderId, Guid userId, EUserType userType);

}