using RestAPI.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace RestAPI.Models
{
    public class Product
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string Brand { get; set; }
        [Required]
        public string Model { get; set; }

        public string? Description;

        public ProductType ProductType { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }


        public Product() { }

        public Product(ProductCreate product, ProductType productType, Guid updateUserID)
        {
            Id = Id == Guid.Empty ? Guid.NewGuid() : Id;
            Brand = product.Brand;
            Model = product.Model;
            Description = product.Description;
            ProductType = productType;
            CreatedAt = DateTime.Now;
            CreatedBy= updateUserID;
        }
    }


    public class ProductUpdate
    {
        public EProductType? Type { get; set; }
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public string? Description;
    }
    public class ProductCreate
    {
        [Required]
        public EProductType Type { get; set; }

        [Required]
        public string Brand { get; set; }
        [Required]
        public string Model { get; set; }

        public string? Description;

    }

    public class ProductResponse
    {
        public Guid Id { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public EProductType Type { get; set; }
        
        public ProductResponse(Product product)
        {
            Id = product.Id;
            Brand = product.Brand;
            Model = product.Model;
            Type = product.ProductType.Type;
        }
    }
}
