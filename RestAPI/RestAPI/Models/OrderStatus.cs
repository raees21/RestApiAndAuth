using RestAPI.Common.Enums;

namespace RestAPI.Models;

public class OrderStatus
{
    public int Id { get; set; }
    public EOrderStatus Status { get; set; }
}