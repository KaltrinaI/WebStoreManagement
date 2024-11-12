using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebStoreApp.DTOs;
using WebStoreApp.Services.Interfaces;

namespace WebStoreApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin,AdvancedUser,SimpleUser")]
        public async Task<ActionResult> AddProduct([FromBody] ProductDTO productDto)
        {
            await _productService.AddProduct(productDto);
            return Ok();
        }

        [HttpGet("{productId}")]
        public async Task<ActionResult<ProductDTO>> GetProductById(int productId)
        {
            var product = await _productService.GetProductById(productId);
            if (product == null)
            {
                return NotFound("Product not found.");
            }
            return Ok(product);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetAllProducts()
        {
            var products = await _productService.GetAllProducts();
            return Ok(products);
        }

        [HttpGet("out-of-stock")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetOutOfStockProducts()
        {
            var products = await _productService.GetOutOfStockProducts();
            return Ok(products);
        }

        [HttpGet("brand/{brand}")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProductsByBrand(string brand)
        {
            var products = await _productService.GetProductsByBrand(brand);
            return Ok(products);
        }

        [HttpGet("category/{category}")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProductsByCategory(string category)
        {
            var products = await _productService.GetProductsByCategory(category);
            return Ok(products);
        }

        [HttpGet("color/{color}")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProductsByColor(string color)
        {
            var products = await _productService.GetProductsByColor(color);
            return Ok(products);
        }

        [HttpGet("gender/{gender}")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProductsByGender(string gender)
        {
            var products = await _productService.GetProductsByGender(gender);
            return Ok(products);
        }

        [HttpGet("size/{size}")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProductsBySize(string size)
        {
            var products = await _productService.GetProductsBySize(size);
            return Ok(products);
        }

        [HttpGet("with-discount")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProductsWithDiscount()
        {
            var products = await _productService.GetProductsWithDiscount();
            return Ok(products);
        }

        [HttpPut("{productId}")]
        [Authorize(Roles = "Admin,AdvancedUser,SimpleUser")]
        public async Task<IActionResult> UpdateProduct(int productId, [FromBody] ProductDTO productDto)
        {
            await _productService.UpdateProduct(productId, productDto);
            return NoContent();
        }

        [HttpDelete("{productId}")]
        [Authorize(Roles = "Admin,AdvancedUser,SimpleUser")]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            await _productService.DeleteProduct(productId);
            return NoContent();
        }

        [HttpGet("quantity/{productId}")]
        [Authorize(Roles = "Admin,AdvancedUser,SimpleUser")]
        public async Task<ActionResult<ProductQuantityDTO>> GetRealTimeProductQuantity(int productId)
        {
            try
            {
                var quantityInfo = await _productService.GetRealTimeProductQuantity(productId);
                return Ok(quantityInfo);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { Error = ex.Message });
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> AdvancedProductSearch(
            [FromQuery] string? category,
            [FromQuery] string? gender,
            [FromQuery] string? brand,
            [FromQuery] double? minPrice,
            [FromQuery] double? maxPrice,
            [FromQuery] string? size,
            [FromQuery] string? color,
            [FromQuery] bool? inStock)
        {
            var products = await _productService.AdvancedProductSearch(category, gender, brand, minPrice, maxPrice, size, color, inStock);
            return Ok(products);
        }

    }
}
