using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Swashbuckle.AspNetCore.Annotations;
using WebStoreApp.DTOs;
using WebStoreApp.Exceptions;
using WebStoreApp.Models;
using WebStoreApp.Services.Helpers;
using WebStoreApp.Services.Interfaces;

namespace WebStoreApp.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/products")]
    [ApiVersion("1.0")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly LinkHelper _linkHelper;


        public ProductController(IProductService productService, LinkHelper linkHelper)
        {
            _productService = productService;
            _linkHelper = linkHelper;
        }

        [HttpPost]
        [Authorize(Roles = "Admin,AdvancedUser,SimpleUser")]
        [SwaggerOperation(Summary = "Adds a new product")]
        [SwaggerResponse(201, "Product added successfully")]
        [SwaggerResponse(400, "Invalid input")]
        [SwaggerResponse(401, "Unauthorized - User is not authenticated")]
        [SwaggerResponse(403, "Forbidden - User does not have permission")]
        public async Task<ActionResult> AddProduct([FromBody] ProductDTO productDto)
        {
            try
            {
                if (!ModelState.IsValid || productDto == null)
                {
                    return BadRequest(new { message = "Invalid product data." });
                }

                await _productService.AddProduct(productDto);
                return StatusCode(201, new { message = "Product added successfully." });
            }
            catch (ServiceException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred while adding the product.", error = ex.Message });
            }
        }


        [HttpGet("{productId}")]
        [SwaggerOperation(Summary = "Gets a product by ID")]
        [SwaggerResponse(200, "Success", typeof(ProductDTO))]
        [SwaggerResponse(404, "Product not found")]
        public async Task<ActionResult<ProductDTO>> GetProductById(int productId)
        {
            try
            {
                var product = await _productService.GetProductById(productId);
                if (product == null)
                {
                    return NotFound(new { message = "Product not found." });
                }

                var userRoles = HttpContext.User.FindAll(System.Security.Claims.ClaimTypes.Role)
                                .Select(role => role.Value)
                                .ToList();

                product.Links = _linkHelper.GenerateProductLinksForSingleProduct(
                    HttpContext,
                    product.Id,
                    product.CategoryName,
                    product.BrandName,
                    product.GenderName,
                    product.SizeName,
                    product.ColorName,
                    userRoles);

                return Ok(product);
            }
            catch (ServiceException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred while retrieving the product.", error = ex.Message });
            }
        }


        [HttpGet]
        [SwaggerOperation(Summary = "Retrieves all products")]
        [SwaggerResponse(200, "Success", typeof(IEnumerable<ProductDTO>))]
        [SwaggerResponse(404, "No products found")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetAllProducts()
        {
            try
            {
                var products = await _productService.GetAllProducts();
                if (!products.Any())
                {
                    return NotFound(new { message = "No products found." });
                }

                var userRoles = HttpContext.User.FindAll(System.Security.Claims.ClaimTypes.Role)
                               .Select(role => role.Value)
                               .ToList();

                foreach (var product in products)
                {
                    product.Links = _linkHelper.GenerateProductLinksForAllProducts(
                        HttpContext,
                        product.Id,
                        product.CategoryName,
                        userRoles
                    );
                }
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving products.", error = ex.Message });
            }
        }

        [HttpGet("out-of-stock")]
        [SwaggerOperation(Summary = "Retrieves all out-of-stock products")]
        [SwaggerResponse(200, "Success", typeof(IEnumerable<ProductDTO>))]
        [SwaggerResponse(404, "No out-of-stock products found")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetOutOfStockProducts()
        {
            try
            {
                var products = await _productService.GetOutOfStockProducts();
                if (!products.Any())
                {
                    return NotFound(new { message = "No out-of-stock products found." });
                }
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving out-of-stock products.", error = ex.Message });
            }
        }

        [HttpGet("brand/{brand}")]
        [SwaggerOperation(Summary = "Retrieves products by brand")]
        [SwaggerResponse(200, "Success", typeof(IEnumerable<ProductDTO>))]
        [SwaggerResponse(404, "No products found for the specified brand")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProductsByBrand(string brand)
        {
            try
            {
                var products = await _productService.GetProductsByBrand(brand);
                if (!products.Any())
                {
                    return NotFound(new { message = $"No products found for brand: {brand}." });
                }
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving products by brand.", error = ex.Message });
            }
        }

        [HttpGet("category/{category}")]
        [SwaggerOperation(Summary = "Retrieves products by category")]
        [SwaggerResponse(200, "Success", typeof(IEnumerable<ProductDTO>))]
        [SwaggerResponse(404, "No products found for the specified category")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProductsByCategory(string category)
        {
            try
            {
                var products = await _productService.GetProductsByCategory(category);
                if (!products.Any())
                {
                    return NotFound(new { message = $"No products found for category: {category}." });
                }
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving products by category.", error = ex.Message });
            }
        }

        [HttpGet("color/{color}")]
        [SwaggerOperation(Summary = "Retrieves products by color")]
        [SwaggerResponse(200, "Success", typeof(IEnumerable<ProductDTO>))]
        [SwaggerResponse(404, "No products found for the specified color")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProductsByColor(string color)
        {
            try
            {
                var products = await _productService.GetProductsByColor(color);
                if (!products.Any())
                {
                    return NotFound(new { message = $"No products found for color: {color}." });
                }
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving products by color.", error = ex.Message });
            }
        }

        [HttpGet("gender/{gender}")]
        [SwaggerOperation(Summary = "Retrieves products by gender")]
        [SwaggerResponse(200, "Success", typeof(IEnumerable<ProductDTO>))]
        [SwaggerResponse(404, "No products found for the specified gender")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProductsByGender(string gender)
        {
            try
            {
                var products = await _productService.GetProductsByGender(gender);
                if (!products.Any())
                {
                    return NotFound(new { message = $"No products found for gender: {gender}." });
                }
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving products by gender.", error = ex.Message });
            }
        }

        [HttpGet("size/{size}")]
        [SwaggerOperation(Summary = "Retrieves products by size")]
        [SwaggerResponse(200, "Success", typeof(IEnumerable<ProductDTO>))]
        [SwaggerResponse(404, "No products found for the specified size")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProductsBySize(string size)
        {
            try
            {
                var products = await _productService.GetProductsBySize(size);
                if (!products.Any())
                {
                    return NotFound(new { message = $"No products found for size: {size}." });
                }
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving products by size.", error = ex.Message });
            }
        }

        [HttpGet("with-discount")]
        [SwaggerOperation(Summary = "Retrieves products with discounts")]
        [SwaggerResponse(200, "Success", typeof(IEnumerable<ProductDTO>))]
        [SwaggerResponse(404, "No products with discounts found")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProductsWithDiscount()
        {
            try
            {
                var products = await _productService.GetProductsWithDiscount();
                if (!products.Any())
                {
                    return NotFound(new { message = "No products with discounts found." });
                }
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving products with discounts.", error = ex.Message });
            }
        }


        [HttpPut("{productId}")]
        [Authorize(Roles = "Admin,AdvancedUser,SimpleUser")]
        [SwaggerOperation(Summary = "Updates an existing product")]
        [SwaggerResponse(200, "Product updated successfully")]
        [SwaggerResponse(400, "Invalid input")]
        [SwaggerResponse(401, "Unauthorized - User is not authenticated")]
        [SwaggerResponse(403, "Forbidden - User does not have permission")]
        [SwaggerResponse(404, "Product not found")]
        public async Task<IActionResult> UpdateProduct(int productId, [FromBody] ProductDTO productDto)
        {
            try
            {
                if (!ModelState.IsValid || productDto == null)
                {
                    return BadRequest(new { message = "Invalid product data." });
                }

                var existingProduct = await _productService.GetProductById(productId);
                if (existingProduct == null)
                {
                    return NotFound(new { message = "Product not found." });
                }

                await _productService.UpdateProduct(productId, productDto);
                return Ok(new { message = $"Product with ID {productId} updated successfully." });
            }
            catch (ServiceException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred while updating the product.", error = ex.Message });
            }
        }


        [HttpDelete("{productId}")]
        [Authorize(Roles = "Admin,AdvancedUser,SimpleUser")]
        [SwaggerOperation(Summary = "Deletes a product")]
        [SwaggerResponse(204, "Product deleted successfully")]
        [SwaggerResponse(401, "Unauthorized - User is not authenticated")]
        [SwaggerResponse(403, "Forbidden - User does not have permission")]
        [SwaggerResponse(404, "Product not found")]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            try
            {
                var existingProduct = await _productService.GetProductById(productId);
                if (existingProduct == null)
                {
                    return NotFound(new { message = "Product not found." });
                }

                await _productService.DeleteProduct(productId);
                return NoContent();
            }
            catch (ServiceException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred while deleting the product.", error = ex.Message });
            }
        }


        [HttpGet("quantity/{productId}")]
        [Authorize(Roles = "Admin,AdvancedUser,SimpleUser")]
        [SwaggerOperation(Summary = "Gets real-time product quantity")]
        [SwaggerResponse(200, "Success", typeof(ProductQuantityDTO))]
        [SwaggerResponse(401, "Unauthorized - User is not authenticated")]
        [SwaggerResponse(403, "Forbidden - User does not have permission")]
        [SwaggerResponse(404, "Product not found")]
        public async Task<ActionResult<ProductQuantityDTO>> GetRealTimeProductQuantity(int productId)
        {
            try
            {
                var quantityInfo = await _productService.GetRealTimeProductQuantity(productId);
                if (quantityInfo == null)
                {
                    return NotFound(new { message = "Product not found." });
                }
                return Ok(quantityInfo);
            }
            catch (ServiceException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred while retrieving the product quantity.", error = ex.Message });
            }
        }


        [HttpGet("search")]
        [SwaggerOperation(Summary = "Searches products based on multiple filters")]
        [SwaggerResponse(200, "Success", typeof(IEnumerable<ProductDTO>))]
        [SwaggerResponse(400, "Invalid input parameters")]
        [SwaggerResponse(404, "No products found matching the criteria")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> AdvancedProductSearch(
            [FromQuery] string? category,
            [FromQuery] string? gender,
            [FromQuery] string? brand,
            [FromQuery] string? size,
            [FromQuery] string? color,
            [FromQuery] bool? inStock,
            [FromQuery] double? minPrice,
            [FromQuery] double? maxPrice)
        {
            try
            {
                var products = await _productService.AdvancedProductSearch(category, gender, brand, size, color, inStock,minPrice,maxPrice);

                if (!products.Any())
                {
                    return NotFound(new { message = "No products found matching the specified criteria." });
                }

                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while searching for products.", error = ex.Message });
            }
        }

        [HttpGet("price-range")]
        [SwaggerOperation(Summary = "Finds products within a specified price range")]
        [SwaggerResponse(200, "Success", typeof(IEnumerable<ProductDTO>))]
        [SwaggerResponse(400, "Invalid price range")]
        [SwaggerResponse(404, "No products found within the specified price range")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> FindByPriceRange(
            [FromQuery] double minPrice,
            [FromQuery] double maxPrice)
        {
            try
            {
                if (minPrice < 0 || maxPrice < 0 || minPrice > maxPrice)
                {
                    return BadRequest(new { message = "Invalid price range. Ensure minPrice <= maxPrice and both are non-negative." });
                }

                var products = await _productService.FindByPriceRange(minPrice, maxPrice);

                if (!products.Any())
                {
                    return NotFound(new { message = "No products found within the specified price range." });
                }

                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving products by price range.", error = ex.Message });
            }
        }
    }
}
