using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestAPI.Common.Enums;
using RestAPI.Models;
using RestAPI.Services;

namespace RestAPI.Controllers
{
    [ApiController]
    [Route("api/v1/product-variants")]
    [Produces("application/json")]

    public class ProductVariantController : ControllerBase
    {
        private readonly IProductVariantService _productVariantService;
        private readonly IHttpContextAccessor _httpcontextAccessor;

        public ProductVariantController(IProductVariantService productVariantService, IHttpContextAccessor httpContextAccessor)
        {
            _productVariantService = productVariantService;
            _httpcontextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Get all product variants
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProductVariantResponse>), 200)]
        public async Task<ActionResult<IEnumerable<ProductVariant>>> GetProductVariants(
            string? color,
            double? priceMax,
            double? priceMin
            )
        {
            IEnumerable<ProductVariantResponse> productVariants = await _productVariantService.GetAllProductVariants(color, priceMax, priceMin);
            return StatusCode(200, productVariants);
        }

        /// <summary>
        /// Get a single product variants
        /// </summary>
        [HttpGet("{productVariantId}")]
        [ProducesResponseType(typeof(ProductVariantResponse), 200)]
        public async Task<ActionResult<ProductVariantResponse>> GetProductVariant(Guid productVariantId)
        {
            ProductVariantResponse productVariant = await _productVariantService.GetProductVariant(productVariantId);
            
            return StatusCode(200, productVariant);
        }

        /// <summary>
        /// Create a new product variant
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(typeof(ProductVariantResponse), 200)]
        public async Task<ActionResult<ProductVariantResponse>> PostCreateProductVariant(ProductVariantCreate productVariant)
        {
            Guid userId = new Guid(
                _httpcontextAccessor.HttpContext.User.Claims.First(c => c.Type == "UserId").Value
            );

            ProductVariantResponse createdProductVariant = await _productVariantService.CreateProductVariant(productVariant, userId);

            return StatusCode(201, createdProductVariant);
        }

        /// <summary>
        /// Remove a product variant
        /// </summary>
        [HttpDelete("{productVariantId}")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> DeleteProductVariant(Guid productVariantId)
        {
            await _productVariantService.DeleteProductVariant(productVariantId);

            return StatusCode(201);
        }

        /// <summary>
        /// Update a product variant
        /// </summary>
        [HttpPut("{productVariantId}")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> UpdateProductVariant(Guid productVariantId, ProductVariantUpdate productVarient)
        {
            await _productVariantService.UpdateProductVariant(productVariantId, productVarient);

            return StatusCode(200);
        }

    }
}
