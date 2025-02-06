using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using WebStoreApp.Data;
using WebStoreApp.Models;

namespace WebStoreApp.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/solr")]
    [ApiVersion("1.0")]
    public class SolrController : ControllerBase
    {
        private readonly SolrService _solrService;
        private readonly AppDbContext _context;

        public SolrController(SolrService solrService, AppDbContext context)
        {
            _solrService = solrService;
            _context = context;
        }

        /// <summary>
        /// Synchronizes all products from the database to Solr index.
        /// </summary>
        [HttpPost("sync")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Synchronizes all products from the database to Solr index")]
        [SwaggerResponse(200, "Products successfully synchronized", typeof(object))]
        [SwaggerResponse(401, "Unauthorized - User is not authenticated")]
        [SwaggerResponse(403, "Forbidden - User does not have permission")]
        [SwaggerResponse(500, "Internal server error occurred during synchronization", typeof(object))]
        public async Task<IActionResult> SyncProducts()
        {
            try
            {
                var products = await _context.Products
                    .Include(p => p.Brand)
                    .Include(p => p.Category)
                    .Include(p => p.Color)
                    .Include(p => p.Size)
                    .Include(p => p.Gender)
                    .ToListAsync();

                foreach (var product in products)
                {
                    await _solrService.AddOrUpdateProduct(product);
                }

                return Ok(new { message = $"Synchronized {products.Count} products" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Indexes a single product in Solr.
        /// </summary>
        [HttpPost("index")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Indexes a single product in Solr")]
        [SwaggerResponse(200, "Product successfully indexed", typeof(object))]
        [SwaggerResponse(400, "Invalid product data", typeof(object))]
        [SwaggerResponse(401, "Unauthorized - User is not authenticated")]
        [SwaggerResponse(403, "Forbidden - User does not have permission")]
        public async Task<IActionResult> IndexProduct([FromBody] Product product)
        {
            if (product == null)
                return BadRequest(new { error = "Product data cannot be null" });

            await _solrService.AddOrUpdateProduct(product);
            return Ok(new { message = "Product indexed successfully" });
        }

        /// <summary>
        /// Searches for products using various criteria.
        /// </summary>
        [HttpGet("search")]
        [SwaggerOperation(Summary = "Searches for products using various criteria")]
        [SwaggerResponse(200, "Returns the list of matching products", typeof(List<Product>))]
        [SwaggerResponse(400, "Invalid search parameters", typeof(object))]
        public async Task<IActionResult> SearchProducts(
            [FromQuery] string query,
            [FromQuery] int? categoryId,
            [FromQuery] int? brandId,
            [FromQuery] int? colorId,
            [FromQuery] int? sizeId,
            [FromQuery] int? genderId,
            [FromQuery] double? minPrice,
            [FromQuery] double? maxPrice,
            [FromQuery] bool fuzzy = false)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest(new { error = "Search query cannot be empty" });

            var results = await _solrService.SearchProducts(
                query, categoryId, brandId, colorId, sizeId, genderId, minPrice, maxPrice, fuzzy);

            return Ok(results);
        }
    }
}

