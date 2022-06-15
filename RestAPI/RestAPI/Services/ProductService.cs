using Microsoft.EntityFrameworkCore;
using RestAPI.Common.Enums;
using RestAPI.DataAccess;
using RestAPI.Models;
using System.Net;

namespace RestAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly ILogger<ProductService> _logger;
        private readonly DataContext _dataContext;
        public ProductService(ILogger<ProductService> logger, DataContext dataContext)
        {
            _logger = logger;
            _dataContext = dataContext;
        }

        public async Task<ProductResponse> CreateProduct(ProductCreate newProduct, Guid userId)
        {
            _logger.LogInformation($"Request to create new product. Brand: {newProduct.Brand} Model: {newProduct.Model}");

            ProductType productType = FetchProductType(newProduct.Type);

            Product product = new Product(newProduct, productType, userId);
            
            var createdProduct = await _dataContext.Products.AddAsync(product);

            await _dataContext.SaveChangesAsync();

            _logger.LogInformation($"Product #{createdProduct.Entity.Id} added");

            return MapProductResponse(createdProduct.Entity);
        }


        public async Task<IEnumerable<ProductResponse>> GetAllProducts(string? brand, string? model)
        {
            _logger.LogInformation($"Request for all products");
            
            IEnumerable<Product> products = await _dataContext.Products
                .Include(p => p.ProductType)
                .Where(p => (brand == null) | p.Brand.ToLower().Contains(brand))
                .Where(p => (model == null) | p.Model.ToLower().Contains(model))
                .ToListAsync();
            
            _logger.LogInformation($"{products.Count()} product records found");

            return MapProductsResponse(products);
        }
        
        private IEnumerable<ProductResponse> MapProductsResponse(IEnumerable<Product> products)
        {
            return products.Select(p => MapProductResponse(p)).ToList();
        }

        private ProductResponse MapProductResponse(Product product)
        {
            return new ProductResponse(product);
        }

        public async Task<ProductResponse> GetProduct(Guid productId)
        {
            _logger.LogInformation($"Request for product #{productId}");

            Product product = FetchProduct(productId);
            
            _logger.LogInformation( $"Record for product #{productId} found");

            return MapProductResponse(product);
        }

        public async Task UpdateProduct(Guid productId, ProductUpdate productUpdate)
        {
            _logger.LogInformation($"Request to update product #{productId}");

            Product product = FetchProduct(productId);

            product.Brand = productUpdate.Brand ?? product.Brand;
            product.Model = productUpdate.Model ?? product.Model;
            product.Description = productUpdate.Description ?? product.Description;
            product.ProductType = productUpdate.Type.HasValue ? FetchProductType(productUpdate.Type.Value) : product.ProductType;
            
            await _dataContext.SaveChangesAsync();
            
            _logger.LogInformation($"Product #{productId} updated");
        }

        public async Task DeleteProduct(Guid productId) 
        {
            _logger.LogInformation($"Request to delete product #{productId}");

            Product product = await _dataContext.Products.FirstOrDefaultAsync(p => p.Id == productId);

            if (product is null)
            {
                throw new HttpStatusException(HttpStatusCode.NotFound, "Product could not be found");
            }

            _dataContext.Products.Remove(product);
            
            await _dataContext.SaveChangesAsync();
            
            _logger.LogInformation($"Product #{productId} and all linked product variants deleted");

        }

        private ProductType FetchProductType(EProductType type)
        {
            ProductType productType = _dataContext.ProductTypes.FirstOrDefault(productType => productType.Type == type);

            if (productType is null)
            {
                throw new HttpStatusException(HttpStatusCode.NotFound, "Product type not found");
            }

            return productType;
        }

        private Product FetchProduct(Guid productId)
        {
            Product product = _dataContext.Products
                .Include(p => p.ProductType)
                .FirstOrDefault(p => p.Id == productId);
            
            if (product is null)
            {
                throw new HttpStatusException(HttpStatusCode.NotFound, "Product could not be found");
            }

            return product;
        }
    }
}
