using System.ComponentModel.DataAnnotations;
using RestAPI.Common.Enums;

namespace RestAPI.Models;

public class OrderItem
{
    [Required]
    public Guid OrderId { get; set; }
    public Order Order { get; set; }
    [Required]
    public Guid ProductVariantId { get; set; }
    public ProductVariant ProductVariant { get; set; }
    public OrderItem() {}
}

public class OrderItemResponse
{
    public Guid Id { get; set; }
    public string Brand { get; set; }
    public string Model { get; set; }
    public string Color { get; set; }
    public string Size { get; set; }
    public EFootSide Side { get; set; }
    public double Price { get; set; }

    public OrderItemResponse(ProductVariant productVariant)
    {
        Id = productVariant.Id;
        Brand = productVariant.Product.Brand;
        Model = productVariant.Product.Model;
        Color = productVariant.Color;
        Price = productVariant.Price;
        Size = $"{productVariant.ShoeSize.Code} {productVariant.ShoeSize.Size}";
        Side = productVariant.FootSide.Side;
    }
}