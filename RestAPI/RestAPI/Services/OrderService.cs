using System.Net;
using Microsoft.EntityFrameworkCore;
using RestAPI.Common.Constants;
using RestAPI.Common.Enums;
using RestAPI.DataAccess;
using RestAPI.Models;

namespace RestAPI.Services;

public class OrderService: IOrderService
{
    private readonly ILogger<OrderService> _logger;
    private readonly DataContext _dataContext;

    public OrderService(
        ILogger<OrderService> logger, 
        DataContext dataContext
    )
    {
        _logger = logger;
        _dataContext = dataContext;
    }
    
    public async Task<OrderResponse> CreateOrder(OrderCreate newOrder, Guid userId)
    {
        _logger.LogInformation($"Request to create new order for user #{userId}");

        OrderType orderType = FetchOrderType(newOrder.Type);
        Address? address = orderType.Type == EOrderType.Delivery ? FetchAddress(newOrder.AddressId) : null;

        if (orderType.Type == EOrderType.Delivery && address is null)
        {
            throw new HttpStatusException(HttpStatusCode.NotFound, $"Address #{newOrder.AddressId} could not be found");
        }

        OrderStatus orderStatus = FetchOrderStatus(EOrderStatus.Pending);
        UserProfile userProfile = FetchUserProfile(userId);

        Order order = new Order(userProfile, orderStatus, orderType, address);
        
        var createdOrder = await _dataContext.Orders.AddAsync(order);
        
        InsertOrderItems(createdOrder.Entity, newOrder.ProductVariantIds);
        
        await _dataContext.SaveChangesAsync();
        
        _logger.LogInformation($"Order #{createdOrder.Entity.Id} created for user #{createdOrder.Entity.UserProfile.Id}");

        return MapOrderResponse(createdOrder.Entity);
    }
    
    public async Task<IEnumerable<OrderResponse>> GetAllOrders(
        IEnumerable<EOrderStatus> statuses, 
        IEnumerable<EOrderType> types,
        Guid? userId = null
        )
    {
        _logger.LogInformation($"Request for all orders");

        IEnumerable<Order> orders = _dataContext.Orders
            .Include(o => o.StatusUpdates)
            .Include(o => o.UserProfile)
            .Include(o => o.OrderType)
            .Include(o => o.Address)
            .AsEnumerable()
            .Where(o => (userId == null) | o.UserProfile.Id == userId)
            .Where(o => (types.Count() == 0) || types.Contains(o.OrderType.Type))
            .Where(o =>
            {
                OrderStatusUpdate? lastStatusUpdate = _dataContext.OrderStatusUpdates
                    .Include(su => su.OrderStatus)
                    .Where(su => su.OrderId == o.Id)
                    .OrderByDescending(su => su.CreatedAt)
                    .FirstOrDefault();
                
                return (statuses.Count() == 0) || statuses.Contains(lastStatusUpdate.OrderStatus.Status);
            });
        
        _logger.LogInformation($"{orders.Count()} order records found");

        return orders.Count() > 0 ? MapOrdersResponse(orders) : new List<OrderResponse>() {};
    }

    public async Task<OrderResponse> GetOrder(Guid orderId, Guid userId, EUserType userType)
    {
        _logger.LogInformation($"Request for order #{orderId}");

        Order order = FetchOrder(orderId);

        if (order.UserProfile.Id != userId && userType != EUserType.Administrator)
        {
            throw new HttpStatusException(HttpStatusCode.Unauthorized, "Unauthorized");
        }

        _logger.LogInformation( $"Record for order #{orderId} found");

        return MapOrderResponse(order);
    }

    public async Task UpdateOrderStatus(Guid orderId, EOrderStatus status, Guid userId)
    {
        _logger.LogInformation($"Request to update order #{orderId}");

        Order order = FetchOrder(orderId);

        OrderStatusUpdate lastStatusUpdate = order.StatusUpdates.MaxBy(su => su.CreatedAt);
        
        ValidateStatusUpdate(lastStatusUpdate, status, order.OrderType.Type);

        OrderStatus nextStatus = FetchOrderStatus(status);

        OrderStatusUpdate newUpdate = new OrderStatusUpdate(order.Id, nextStatus, userId);
        
        order.StatusUpdates.Add(newUpdate);

        await _dataContext.SaveChangesAsync();
        
        _logger.LogInformation($"Order #{orderId} updated by #{userId}");
    }

    public async Task<IEnumerable<OrderStatusUpdateResponse>> GetAllOrderStatusUpdates(
        Guid orderId, 
        Guid userID,
        EUserType userRole
        )
    {
        _logger.LogInformation($"Request for status update history for order #{orderId}");

        Order order = FetchOrder(orderId);

        if (order.UserProfile.Id != userID && userRole != EUserType.Administrator)
        {
            throw new HttpStatusException(HttpStatusCode.Unauthorized, "Unauthorized");
        }
        
        _logger.LogInformation($"{order.StatusUpdates.Count()} status update records for order #{orderId} found");
        
        return MapOrderStatusUpdatesResponse(order.StatusUpdates);
    }

    private IEnumerable<OrderStatusUpdateResponse> MapOrderStatusUpdatesResponse(IEnumerable<OrderStatusUpdate> statusUpdates)
    {
        return statusUpdates
            .Select(statusUpdate => MapOrderStatusUpdateResponse(statusUpdate))
            .OrderByDescending(statusUpdate => statusUpdate.UpdatedAt);
    }
    
    private OrderStatusUpdateResponse MapOrderStatusUpdateResponse(OrderStatusUpdate statusUpdate)
    {
        OrderStatusUpdate orderStatusUpdate = _dataContext.OrderStatusUpdates
            .Include(osu => osu.OrderStatus)
            .FirstOrDefault(osu => osu.Id == statusUpdate.Id);
        
        return new OrderStatusUpdateResponse
        {
            Status = orderStatusUpdate.OrderStatus.Status,
            UpdatedAt = statusUpdate.CreatedAt
        };
    }
    private Order FetchOrder(Guid orderId)
    {
        Order? order = _dataContext.Orders
            .Include(o => o.StatusUpdates)
            .Include(o => o.UserProfile)
            .Include(o => o.OrderType)
            .Include(o => o.Address)
            .FirstOrDefault(o => o.Id == orderId);

        if (order is null)
        {
            throw new HttpStatusException(HttpStatusCode.NotFound, "Order not found");
        }

        return order;
    }

    private void InsertOrderItems(Order order, IEnumerable<Guid> productVariantIds)
    {
        IEnumerable<ProductVariant> productVariants = _dataContext.ProductVariants.Where(pv => productVariantIds.Contains(pv.Id));
        
        foreach(Guid productVariantId in productVariantIds)
        {
            ProductVariant productVariant = productVariants.FirstOrDefault(pv => pv.Id == productVariantId);
            
            if (productVariant is null)
            {
                throw new HttpStatusException(HttpStatusCode.NotFound, "Product variant could not be found");
            }
            
            _logger.LogInformation($"Adding ProductVariant #{productVariant.Id} to Order #{order.Id}");
            _dataContext.OrderItems.Add(new OrderItem
            {
                Order = order,
                ProductVariant = productVariant
            });
            
            productVariant.Quantity--;
            _logger.LogInformation($"ProductVariant #{productVariant.Id} items in stock reduced to {productVariant.Quantity}");
        };
    }
    private void ValidateStatusUpdate(OrderStatusUpdate latestStatusUpdate, EOrderStatus nextStatus, EOrderType orderType)
    {
        OrderStatusUpdate orderStatusUpdate = _dataContext.OrderStatusUpdates
            .Include(osu => osu.OrderStatus)
            .FirstOrDefault(osu => osu.Id == latestStatusUpdate.Id);

        List<EOrderStatus> allowedStatuses = OrderStatusWorkflow.AllowedStatusMap[orderStatusUpdate.OrderStatus.Status];
        List<EOrderStatus> disallowedStatuses = OrderStatusWorkflow.DisallowedStatusMap[orderType];
        
        if (!allowedStatuses.Contains(nextStatus) || disallowedStatuses.Contains(nextStatus))
        {
            throw new HttpStatusException(
                HttpStatusCode.BadRequest,
                $"Cannot update to status:{nextStatus.ToString()} " +
                $"from status:{orderStatusUpdate.OrderStatus.Status.ToString()} " +
                $"for order type:{orderType.ToString()}"
            );
        }
    }

    private IEnumerable<OrderResponse> MapOrdersResponse(IEnumerable<Order> orders)
    {
        return orders.Select(order => MapOrderResponse(order)).ToList();
    }
    
    private OrderResponse MapOrderResponse(Order order)
    {
        OrderStatusUpdate lastUpdate = _dataContext.OrderStatusUpdates
            .Include(osu => osu.OrderStatus)
            .Where(osu => order.StatusUpdates.Contains(osu))
            .OrderByDescending(osu => osu.CreatedAt)
            .FirstOrDefault();

        IEnumerable<OrderItem> orderItems = _dataContext.OrderItems
            .Where(oi => oi.OrderId == order.Id)
            .ToList();

        IEnumerable<OrderItemResponse> orderItemResponses = MapOrderItemsResponse(orderItems);

        double total = orderItemResponses.Sum(orderItem => orderItem.Price);

        return new OrderResponse(order, lastUpdate, orderItemResponses, total);
    }

    private IEnumerable<OrderItemResponse> MapOrderItemsResponse(IEnumerable<OrderItem> orderItems)
    {
        return orderItems.Select(orderItem => MapOrderItemResponse(orderItem)).ToList();
    }

    private OrderItemResponse MapOrderItemResponse(OrderItem orderItem)
    {
        ProductVariant productVariant = _dataContext.ProductVariants
            .Include(pv => pv.FootSide)
            .Include(pv => pv.ShoeSize)
            .Include(pv => pv.Product)
            .FirstOrDefault(pv => pv.Id == orderItem.ProductVariantId);

        return new OrderItemResponse(productVariant);
    }
    
    private OrderStatus FetchOrderStatus(EOrderStatus status)
    {
        OrderStatus? orderStatus = _dataContext.OrderStatuses.FirstOrDefault(orderStatus => orderStatus.Status == status);

        if (orderStatus is null)
        {
            throw new HttpStatusException(HttpStatusCode.NotFound, "Invalid order status");
        }

        return orderStatus;
    }
    
    private Address FetchAddress(Guid addressId)
    {
        Address? address = _dataContext.Addresses.FirstOrDefault(address => address.Id == addressId);

        if (address is null)
        {
            throw new HttpStatusException(HttpStatusCode.NotFound, "Address not found");
        }

        return address;
    }
    
    private UserProfile FetchUserProfile(Guid userId)
    {
        UserProfile? userProfile = _dataContext.UserProfiles.FirstOrDefault(userProfile => userProfile.Id == userId);

        if (userProfile is null)
        {
            throw new HttpStatusException(HttpStatusCode.NotFound, "User profile not found");
        }

        return userProfile;
    }
    
    private OrderType FetchOrderType(EOrderType type)
    {
        OrderType? orderType = _dataContext.OrderTypes.FirstOrDefault(orderType => orderType.Type == type);

        if (orderType is null)
        {
            throw new HttpStatusException(HttpStatusCode.NotFound, "Invalid order tpye");
        }

        return orderType;
    }
}