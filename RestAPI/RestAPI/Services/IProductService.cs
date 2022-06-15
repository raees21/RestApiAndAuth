using RestAPI.Models;

namespace RestAPI.Services
{
    public interface IProductService
    {
        public Task<ProductResponse> CreateProduct(ProductCreate product, Guid userId);

        public Task<IEnumerable<ProductResponse>> GetAllProducts(string? brand, string? model);

        public Task DeleteProduct(Guid id);

        public Task UpdateProduct(Guid id, ProductUpdate productUpdate);

        public Task<ProductResponse> GetProduct(Guid id);
    }
}
