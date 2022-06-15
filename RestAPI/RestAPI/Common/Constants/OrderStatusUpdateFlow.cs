using RestAPI.Common.Enums;

namespace RestAPI.Common.Constants;
public static class OrderStatusWorkflow
{
    public static Dictionary<EOrderType, List<EOrderStatus>> DisallowedStatusMap =
        new()
        {
            {
                EOrderType.Collection,
                new List<EOrderStatus>() { EOrderStatus.Shipped, EOrderStatus.Delivered }
            },
            {
                EOrderType.Delivery,
                new List<EOrderStatus>() { EOrderStatus.ReadyForCollection, EOrderStatus.Collected }
            }
        };
    
    public static Dictionary<EOrderStatus, List<EOrderStatus>> AllowedStatusMap =
        new()
        {
            {
                EOrderStatus.Pending,
                new List<EOrderStatus>() { EOrderStatus.Confirmed }
            },
            {
                EOrderStatus.Confirmed,
                new List<EOrderStatus>() { EOrderStatus.InProgress }
            },
            {
                EOrderStatus.InProgress,
                new List<EOrderStatus>() { EOrderStatus.Shipped, EOrderStatus.ReadyForCollection }
            },
            {
                EOrderStatus.Shipped,
                new List<EOrderStatus>() { EOrderStatus.Delivered }
            },
            {
                EOrderStatus.ReadyForCollection,
                new List<EOrderStatus>() { EOrderStatus.Collected }
            }
        };
}
