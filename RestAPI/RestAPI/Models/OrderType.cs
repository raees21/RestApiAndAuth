using RestAPI.Common.Enums;

namespace RestAPI.Models;

public class OrderType
{
    public int Id { get; set; }
    public EOrderType Type { get; set; }
}