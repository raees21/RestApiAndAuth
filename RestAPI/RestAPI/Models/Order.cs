using System.ComponentModel.DataAnnotations;
using RestAPI.Common.Enums;

namespace RestAPI.Models;

public class Order
{
    [Required]
    public Guid Id { get; set; }
    [Required]
    public UserProfile UserProfile { get; set; }
    [Required]
    public OrderType OrderType { get; set; }
    public Address? Address { get; set; }
    public ICollection<OrderStatusUpdate> StatusUpdates { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; }
    public Order() {}

    public Order(UserProfile userProfile, OrderStatus orderStatus, OrderType orderType, Address? address)
    {
        Id = Id == Guid.Empty ? new Guid() : Id;
        UserProfile = userProfile;
        OrderType = orderType;
        Address = address;
        StatusUpdates = new List<OrderStatusUpdate>()
        {
            new(Id, orderStatus, userProfile.Id)
        };
    }
}

public class OrderCreate
{
    [Required]
    public EOrderType Type { get; set; }
    public Guid AddressId { get; set; }
    [Required]
    [MinLength(1)]
    public IEnumerable<Guid> ProductVariantIds { get; set; }
}

public class OrderUpdate
{
    [Required] 
    public EOrderStatus Status { get; set; }
}

public class OrderResponse
{
    public Guid UserId { get; set; }
    public Guid Id { get; set; }
    public AddressResponse? Address { get; set; }
    public EOrderStatus Status { get; set; }
    public DateTime LastUpdate { get; set; }
    public EOrderType Type { get; set; }
    public IEnumerable<OrderItemResponse> Items { get; set; }
    public double Total { get; set; }

    public OrderResponse(Order order, OrderStatusUpdate lastUpdate, IEnumerable<OrderItemResponse> items, double total)
    {
        Id = order.Id;
        Address = order.Address is null ? null : new AddressResponse(order.Address);
        LastUpdate = lastUpdate!.CreatedAt;
        Status = lastUpdate.OrderStatus.Status;
        Type = order.OrderType.Type;
        UserId = order.UserProfile.Id;
        Items = items;
        Total = total;
    }
}