using System.ComponentModel.DataAnnotations;
using RestAPI.Common.Enums;

namespace RestAPI.Models;

public class OrderStatusUpdate
{   
    [Required]
    public Guid Id { get; set; }
    [Required]
    public Guid OrderId { get; set; }
    [Required]
    public OrderStatus OrderStatus { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    
    public OrderStatusUpdate() {}
    public OrderStatusUpdate(Guid orderId, OrderStatus orderStatus, Guid updateUserId)
    {
        Id = new Guid();
        OrderId = orderId;
        OrderStatus = orderStatus;
        CreatedAt = DateTime.Now;
        CreatedBy = updateUserId;
    }
}

public class OrderStatusUpdateResponse
{
    public EOrderStatus Status { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public OrderStatusUpdateResponse() {}
}