using RestAPI.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace RestAPI.Models
{
    public class ProductVariant
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string Color { get; set; }
        [Required]
        public double Price { get; set; }
        [Required]
        public int Quantity { get; set; }


        public FootSide FootSide { get; set; }

        public ShoeSize ShoeSize { get; set; }

        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }

        public Product Product { get; set; }

        public ProductVariant() { }

        public ProductVariant(ProductVariantCreate productVariant, FootSide footSide, ShoeSize code, Guid updateUserID, Product product) 
        {
            Id = Id == Guid.Empty ? Guid.NewGuid() : Id;
            Color = productVariant.Color;
            Price = productVariant.Price;
            Quantity = productVariant.Quantity;
            FootSide = footSide;
            ShoeSize = code;
            CreatedAt = DateTime.Now;
            CreatedBy = updateUserID;
            Product = product;
        }
    }

    public class ProductVariantCreate
    {
        [Required]
        public EFootSide Side { get; set; }

        [Required]
        public ShoeSizeCreate Size { get; set; }
        
        [Required]
        public string Color { get; set; }
        [Required]
        public double Price { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public Guid ProductId { get; set; }
    }

    public class ProductVariantUpdate
    {
        public EFootSide? Side { get; set; }
        public ShoeSizeUpdate? Size { get; set; }
        public string? Color { get; set; }
        public double? Price { get; set; }
        public int? Quantity { get; set; }
        public Guid? ProductId { get; set; }
    }

    public class ProductVariantResponse
    {
        public Guid Id { get; set; }
        public string Color { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }

        public string Size { get; set; } 

        public EFootSide Side { get; set; }

        public string Brand { get; set; }
        public string Model { get; set; }
        public string Description { get; set; }

        public ProductVariantResponse(ProductVariant productVariant)
        {
            Id = productVariant.Id;
            Color = productVariant.Color;
            Price = productVariant.Price;
            Quantity = productVariant.Quantity;
            Side = productVariant.FootSide.Side;
            Size = $"{productVariant.ShoeSize.Code} {productVariant.ShoeSize.Size}";
            Brand = productVariant.Product.Brand;
            Model = productVariant.Product.Model;
            Description = productVariant.Product.Description;

        }

    }
}


