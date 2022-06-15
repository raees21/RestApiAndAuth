using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RestAPI.Common.Enums;
using RestAPI.Common.Helper;
using RestAPI.Models;
using RestAPI.Services;

namespace RestAPI.Controllers;

[ApiController]
[Route("api/v1/orders")]
[Produces("application/json")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public OrderController(IOrderService orderService, IHttpContextAccessor httpContextAccessor) 
    {
        _orderService = orderService;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Create new order for a specified user
    /// </summary>
    [HttpPost]
    [Authorize(Roles="Buyer")]
    [ProducesResponseType(typeof(OrderResponse), 201)]
    public async Task<ActionResult> PostCreateOrder(OrderCreate order)
    {
        Guid userId = OAuth2.UserGuid(_httpContextAccessor);
        
        OrderResponse orderResponse = await _orderService.CreateOrder(order, userId);
        
        return StatusCode(201, orderResponse);
    }
    
    /// <summary>
    /// Get all orders
    /// </summary>
    [HttpGet]
    [Authorize(Roles="Administrator")]
    [ProducesResponseType(typeof(IEnumerable<OrderResponse>), 200)]
    public async Task<ActionResult<IEnumerable<OrderResponse>>> GetAllOrders(
        [FromQuery]List<EOrderStatus>? statuses,
        [FromQuery]List<EOrderType>? types
    )
    {
        IEnumerable<OrderResponse> orders = await _orderService.GetAllOrders(statuses, types);
        
        return StatusCode(200, orders);
    }
    
    /// <summary>
    /// Get a single order
    /// </summary>
    [HttpGet]
    [Route("{orderId}")]
    [Authorize]
    [ProducesResponseType(typeof(OrderResponse), 200)]
    public async Task<ActionResult<OrderResponse>> GetOrder(Guid orderId)
    {
        Guid userId = OAuth2.UserGuid(_httpContextAccessor);
        EUserType userRole = OAuth2.UserRole(_httpContextAccessor);
        
        OrderResponse order = await _orderService.GetOrder(orderId, userId, userRole);

        return StatusCode(200, order);
    }
    
    /// <summary>
    /// Create new status update for an order
    /// </summary>
    [HttpPost]
    [Route("{orderId}/statuses/current")]
    [Authorize(Roles="Administrator")]
    public async Task<ActionResult> PostOrderStatusUpdate(Guid orderId, OrderUpdate orderUpdate)
    {
        Guid userId = OAuth2.UserGuid(_httpContextAccessor);
        
        await _orderService.UpdateOrderStatus(orderId, orderUpdate.Status, userId);
        
        return StatusCode(201);
    }
    
    /// <summary>
    /// Get status update history for an order
    /// </summary>
    [HttpPost]
    [Route("{orderId}/statuses")]
    [Authorize]
    [ProducesResponseType(typeof(IEnumerable<OrderStatusUpdateResponse>), 200)]

    public async Task<ActionResult<IEnumerable<OrderStatusUpdateResponse>>> GetOrderStatusUpdates(Guid orderId)
    {
        Guid userId = OAuth2.UserGuid(_httpContextAccessor);
        EUserType userRole = OAuth2.UserRole(_httpContextAccessor);

        IEnumerable<OrderStatusUpdateResponse> statusUpdates = await _orderService.GetAllOrderStatusUpdates(orderId, userId, userRole);
        
        return StatusCode(201, statusUpdates);
    }
}