using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestAPI.Common.Helper;
using RestAPI.Models;
using RestAPI.Services;

namespace RestAPI.Controllers
{
    [ApiController]
    [Route("api/v1/products")]
    [Produces("application/json")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ProductController(IProductService productService, IHttpContextAccessor httpContextAccessor)
        {
            _productService = productService;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Get all products
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProductResponse>), 200)]
        public async Task<ActionResult<IEnumerable<ProductResponse>>> GetProducts([FromQuery] string? brand, string? model)
        {
            IEnumerable<ProductResponse> products = await _productService.GetAllProducts(brand, model);
            
            return StatusCode(200, products);
        }

        /// <summary>
        /// Get a single product
        /// </summary>
        [HttpGet("{productId}")]
        [ProducesResponseType(typeof(ProductResponse), 200)]

        public async Task<ActionResult<ProductResponse>> GetProduct(Guid productId)
        {
            var product = await _productService.GetProduct(productId);
            
            return StatusCode(200, product);
        }

        /// <summary>
        /// Create a new product
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(typeof(ProductResponse), 201)]
        public async Task<ActionResult<OrderResponse>> PostCreateProduct(ProductCreate product)
        {
            Guid userId = OAuth2.UserGuid(_httpContextAccessor);

            ProductResponse createdProduct = await _productService.CreateProduct(product, userId);

            return StatusCode(201, createdProduct);
        }

        /// <summary>
        /// Remove a product
        /// </summary>
        [HttpDelete("{productId}")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> DeleteProduct(Guid productId) 
        {
            await _productService.DeleteProduct(productId);

            return StatusCode(201);
        }

        /// <summary>
        /// Update a product
        /// </summary>
        [HttpPut("{productId}")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> UpdateProduct(Guid productId, ProductUpdate product)
        {
            await _productService.UpdateProduct(productId, product);

            return StatusCode(204);
        }
    }
}
