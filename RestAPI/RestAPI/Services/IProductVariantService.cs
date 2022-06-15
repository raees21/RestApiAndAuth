using RestAPI.Models;

namespace RestAPI.Services;

public interface IProductVariantService
{  
    public Task<IEnumerable<ProductVariantResponse>> GetAllProductVariants(string? color, double? priceMax, double? priceMin);
    public Task<ProductVariantResponse> GetProductVariant(Guid id);
    public Task<ProductVariantResponse> CreateProductVariant(ProductVariantCreate productVariant, Guid userId);
    public Task DeleteProductVariant(Guid id);
    public Task UpdateProductVariant(Guid id, ProductVariantUpdate productVarient);

}

