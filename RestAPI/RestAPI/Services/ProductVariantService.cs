using System.ComponentModel.DataAnnotations;
using System.Net;
using Microsoft.EntityFrameworkCore;
using RestAPI.Common.Enums;
using RestAPI.DataAccess;
using RestAPI.Models;

namespace RestAPI.Services;

public class ProductVariantService : IProductVariantService
{
    private readonly ILogger<ProductVariantService> _logger;
    private readonly DataContext _dataContext;

    public ProductVariantService(ILogger<ProductVariantService> logger, DataContext dataContext) 
    {
        _logger = logger;
        _dataContext = dataContext;
    }

    public async Task<ProductVariantResponse> CreateProductVariant(ProductVariantCreate newProductVariant, Guid userId)
    {
        _logger.LogInformation($"Request to create new product variant for product #{newProductVariant.ProductId}");

        FootSide footSide = FetchFootSide(newProductVariant.Side);
        ShoeSize shoeSize = FetchShoeSize(newProductVariant.Size.Code, newProductVariant.Size.Size);
        Product product = FetchProduct(newProductVariant.ProductId);

        ProductVariant productVariant = new ProductVariant(newProductVariant, footSide, shoeSize, userId, product);
        
        var createdProductVariant = await _dataContext.ProductVariants.AddAsync(productVariant);

        await _dataContext.SaveChangesAsync();
            
        _logger.LogInformation($"Product variant #{createdProductVariant.Entity.Id} for product #{product.Id} created");

        return MapProductVariantResponse(createdProductVariant.Entity);
    }

    public async Task DeleteProductVariant(Guid productVariantId)
    {
        _logger.LogInformation($"Request to delete product variant #{productVariantId}");

        ProductVariant productVariant = FetchProductVariant(productVariantId);

        _dataContext.ProductVariants.Remove(productVariant);
        
        await _dataContext.SaveChangesAsync();
        
        _logger.LogInformation($"Product variant #{productVariantId} deleted");

    }

    public async Task UpdateProductVariant(Guid productVariantId, ProductVariantUpdate productVariantUpdate)
    {
        _logger.LogInformation($"Request to update product variant #{productVariantId}");

        ProductVariant productVariant = FetchProductVariant(productVariantId);
        
        productVariant.Price = productVariantUpdate.Price ?? productVariant.Price;
        productVariant.Quantity = productVariantUpdate.Quantity ?? productVariant.Quantity;
        productVariant.Color = productVariantUpdate.Color ?? productVariant.Color;

        if (productVariantUpdate.ProductId.HasValue)
        {
            Product product = FetchProduct(productVariantUpdate.ProductId.Value);

            productVariant.Product = product;
        }
        
        if (productVariantUpdate.Side.HasValue)
        {
            FootSide footSide = FetchFootSide(productVariantUpdate.Side.Value);

            productVariant.FootSide = footSide;
        }
        
        if (productVariantUpdate.Size is not null)
        {
            ShoeSize shoeSize = FetchShoeSize(productVariantUpdate.Size.Code, productVariantUpdate.Size.Size);

            productVariant.ShoeSize = shoeSize;
        }

        await _dataContext.SaveChangesAsync();
        
        _logger.LogInformation($"Product variant #{productVariantId} updated");
    }

    public async Task<ProductVariantResponse> GetProductVariant(Guid productVariantId)
    {
        _logger.LogInformation($"Request for product variant #{productVariantId}");

        ProductVariant productVariant = FetchProductVariant(productVariantId);

        _logger.LogInformation( $"Record for product varaint #{productVariantId} found");

        return MapProductVariantResponse(productVariant);
    }

    public async Task<IEnumerable<ProductVariantResponse>> GetAllProductVariants(
        string? color,
        double? priceMax,
        double? priceMin
        )
    {
        _logger.LogInformation($"Request for all product variants");
        
        IEnumerable<ProductVariant> productVariants = await _dataContext.ProductVariants
            .Include(pv => pv.FootSide)
            .Include(pv => pv.ShoeSize)
            .Include(pv => pv.Product)
            .Where(pv => (color == null) | pv.Color.ToLower().Contains(color))
            .Where(pv => (!priceMax.HasValue) | pv.Price <= priceMax.Value)
            .Where(pv => (!priceMin.HasValue) | pv.Price >= priceMin.Value)
            .ToListAsync();
        
        _logger.LogInformation($"{productVariants.Count()} product variant records found");
        
        return MapProductVariantResponse(productVariants);
    }

    private IEnumerable<ProductVariantResponse> MapProductVariantResponse(IEnumerable<ProductVariant> productVariant)
    {
        return productVariant.Select(pv => MapProductVariantResponse(pv)).ToList();
    }

    private ProductVariantResponse MapProductVariantResponse(ProductVariant productVariant)
    {
        return new ProductVariantResponse(productVariant);
    }

    private ProductVariant FetchProductVariant(Guid productVariantId)
    {
        ProductVariant productVarient = _dataContext.ProductVariants
            .Include(pv => pv.Product)
            .Include(pv => pv.ShoeSize)
            .Include(pv => pv.FootSide)
            .FirstOrDefault(pv => pv.Id == productVariantId);

        if (productVarient is null)
        {
            throw new HttpStatusException(HttpStatusCode.NotFound, "Product variant not found");
        }

        return productVarient;
    }
    private FootSide FetchFootSide(EFootSide FootSide)
    {
        FootSide footSide = _dataContext.FootSides.FirstOrDefault(footSide => footSide.Side == FootSide);

        if (footSide is null)
        {
            throw new HttpStatusException(HttpStatusCode.NotFound, "Invalid shoe side");
        }

        return footSide;
    }

    private ShoeSize FetchShoeSize(EShoeSizeCode code, string size)
    {
        ShoeSize shoeSize = _dataContext.ShoeSizes
            .Where(shoeSize => shoeSize.Size == size)
            .FirstOrDefault(shoeSize => shoeSize.Code == code);

        if (shoeSize is null)
        {
            throw new HttpStatusException(HttpStatusCode.BadRequest, "Invalid shoe size");
        }

        return shoeSize;
    }

    private Product FetchProduct(Guid productId)
    {
        Product product = _dataContext.Products
            .FirstOrDefault(id => id.Id == productId);

        if (product is null)
        {
            throw new HttpStatusException(HttpStatusCode.NotFound, "Product not found");
        }

        return product;
    }
}





