using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using WebStoreApp.DTOs;
using WebStoreApp.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using WebStoreApp.Exceptions;

namespace WebStoreApp.Controllers
{
    /// <summary>
    /// Manages discount operations.
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/discounts")]
    [ApiVersion("1.0")]
    [Authorize(Roles = "Admin")]
    public class DiscountController : ControllerBase
    {
        private readonly IDiscountService _discountService;

        public DiscountController(IDiscountService discountService)
        {
            _discountService = discountService;
        }

        /// <summary>
        /// Retrieves a discount by its unique identifier.
        /// </summary>
        /// <param name="discountId">The ID of the discount to retrieve.</param>
        /// <returns>The discount details if found; otherwise, a `404 Not Found` response.</returns>
        [HttpGet("{discountId}")]
        [SwaggerOperation(Summary = "Gets a discount by ID")]
        [SwaggerResponse(200, "Success", typeof(DiscountDTO))]
        [SwaggerResponse(401, "Unauthorized - User is not authenticated")]
        [SwaggerResponse(403, "Forbidden - User does not have permission")]
        [SwaggerResponse(404, "Discount not found")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult<DiscountDTO>> GetDiscountById([Required] int discountId)
        {
            try
            {
                var discount = await _discountService.GetDiscountById(discountId);
                return Ok(discount);
            }
            catch (ServiceException ex)
            {
                return NotFound(new { message = $"Discount with ID {discountId} not found.", error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", error = ex.Message });
            }
        }


        /// <summary>
        /// Retrieves all available discounts.
        /// </summary>
        /// <returns>A list of all discounts.</returns>
        [HttpGet]
        [SwaggerOperation(Summary = "Gets all discounts")]
        [SwaggerResponse(200, "Success", typeof(IEnumerable<DiscountDTO>))]
        [SwaggerResponse(401, "Unauthorized - User is not authenticated")]
        [SwaggerResponse(403, "Forbidden - User does not have permission")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult<IEnumerable<DiscountDTO>>> GetAllDiscounts()
        {
            try
            {
                var discounts = await _discountService.GetAllDiscounts();
                return Ok(discounts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving discounts.", error = ex.Message });
            }
        }


        /// <summary>
        /// Retrieves discounts by their name.
        /// </summary>
        /// <param name="name">The name of the discount(s) to retrieve.</param>
        /// <returns>A list of matching discounts.</returns>
        [HttpGet("by-name/{name}")]
        [SwaggerOperation(Summary = "Gets discounts by name")]
        [SwaggerResponse(200, "Success", typeof(IEnumerable<DiscountDTO>))]
        [SwaggerResponse(401, "Unauthorized - User is not authenticated")]
        [SwaggerResponse(403, "Forbidden - User does not have permission")]
        [SwaggerResponse(404, "No discounts found with the specified name.")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult<IEnumerable<DiscountDTO>>> GetDiscountsByName([Required] string name)
        {
            try
            {
                var discounts = await _discountService.GetDiscountsByName(name);
                if (!discounts.Any())
                {
                    return NotFound(new { message = $"No discounts found with the name '{name}'." });
                }
                return Ok(discounts);
            }
            catch (ServiceException ex)
            {
                return NotFound(new { message = $"No discounts found with the name '{name}'.", error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving discounts.", error = ex.Message });
            }
        }


        /// <summary>
        /// Retrieves discounts within a specified date range.
        /// </summary>
        /// <param name="startDate">The starting date of the range.</param>
        /// <param name="endDate">The ending date of the range.</param>
        /// <returns>A list of discounts within the specified range.</returns>
        [HttpGet("by-date-range")]
        [SwaggerOperation(Summary = "Gets discounts by date range")]
        [SwaggerResponse(200, "Success", typeof(IEnumerable<DiscountDTO>))]
        [SwaggerResponse(400, "Invalid date range provided")]
        [SwaggerResponse(401, "Unauthorized - User is not authenticated")]
        [SwaggerResponse(403, "Forbidden - User does not have permission")]
        [SwaggerResponse(404, "No discounts found for the given date range")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult<IEnumerable<DiscountDTO>>> GetDiscountsByDateRange([Required] DateTime startDate, [Required] DateTime endDate)
        {
            try
            {
                if (startDate > endDate)
                {
                    return BadRequest(new { message = "Start date must be earlier than or equal to end date." });
                }

                var discounts = await _discountService.GetDiscountsByDateRange(startDate, endDate);

                if (!discounts.Any())
                {
                    return NotFound(new { message = $"No discounts found between {startDate} and {endDate}." });
                }

                return Ok(discounts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", error = ex.Message });
            }
        }


        /// <summary>
        /// Adds a new discount.
        /// </summary>
        /// <param name="discountDto">The discount details to add.</param>
        [HttpPost]
        [SwaggerOperation(Summary = "Adds a new discount")]
        [SwaggerResponse(201, "Discount created successfully")]
        [SwaggerResponse(400, "Invalid input")]
        [SwaggerResponse(401, "Unauthorized - User is not authenticated")]
        [SwaggerResponse(403, "Forbidden - User does not have permission")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult> AddDiscount([FromBody, Required] DiscountDTO discountDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _discountService.AddDiscount(discountDto);

                return StatusCode(201, new { message = "Discount created successfully." });
            }
            catch (ServiceException ex)
            {
                return BadRequest(new { message = "Error adding discount.", error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred while adding the discount.", error = ex.Message });
            }
        }


        /// <summary>
        /// Updates an existing discount.
        /// </summary>
        /// <param name="discountId">The ID of the discount to update.</param>
        /// <param name="discountDto">The updated discount details.</param>
        [HttpPut("{discountId}")]
        [SwaggerOperation(Summary = "Updates an existing discount")]
        [SwaggerResponse(200, "Discount updated successfully")]
        [SwaggerResponse(400, "Invalid input")]
        [SwaggerResponse(401, "Unauthorized - User is not authenticated")]
        [SwaggerResponse(403, "Forbidden - User does not have permission")]
        [SwaggerResponse(404, "Discount not found")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult> UpdateDiscount([Required] int discountId, [FromBody, Required] DiscountDTO discountDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _discountService.UpdateDiscount(discountId, discountDto);

                return Ok(new { message = $"Discount with ID {discountId} updated successfully." });
            }
            catch (ServiceException ex)
            {
                return NotFound(new { message = $"Discount with ID {discountId} not found.", error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An unexpected error occurred while updating discount with ID {discountId}.", error = ex.Message });
            }
        }


        /// <summary>
        /// Deletes a discount by its ID.
        /// </summary>
        /// <param name="discountId">The ID of the discount to delete.</param>
        [HttpDelete("{discountId}")]
        [SwaggerOperation(Summary = "Deletes a discount by ID")]
        [SwaggerResponse(204, "Discount deleted successfully")]
        [SwaggerResponse(401, "Unauthorized - User is not authenticated")]
        [SwaggerResponse(403, "Forbidden - User does not have permission")]
        [SwaggerResponse(404, "Discount not found")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult> DeleteDiscount([Required] int discountId)
        {
            try
            {
                await _discountService.DeleteDiscount(discountId);
                return NoContent();
            }
            catch (ServiceException ex)
            {
                return NotFound(new { message = $"Discount with ID {discountId} not found.", error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An unexpected error occurred while deleting discount with ID {discountId}.", error = ex.Message });
            }
        }


        /// <summary>
        /// Applies a discount to a specific product.
        /// </summary>
        /// <param name="productId">The ID of the product.</param>
        /// <param name="discountId">The ID of the discount to apply.</param>
        /// <returns>A success message if the discount is applied.</returns>
        [HttpPost("apply-to-product/{productId}/{discountId}")]
        [SwaggerOperation(Summary = "Applies a discount to a product")]
        [SwaggerResponse(200, "Discount applied successfully", typeof(string))]
        [SwaggerResponse(400, "Invalid input")]
        [SwaggerResponse(401, "Unauthorized - User is not authenticated")]
        [SwaggerResponse(403, "Forbidden - User does not have permission")]
        [SwaggerResponse(404, "Product or discount not found")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult<string>> ApplyDiscountToProduct([Required] int productId, [Required] int discountId)
        {
            try
            {
                if (productId <= 0 || discountId <= 0)
                {
                    return BadRequest(new { message = "Product ID and Discount ID must be valid positive integers." });
                }

                var response = await _discountService.ApplyDiscountToProduct(productId, discountId);
                return Ok(response);
            }
            catch (ServiceException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", error = ex.Message });
            }
        }


        /// <summary>
        /// Applies a discount to all products in a specific category.
        /// </summary>
        /// <param name="categoryName">The name of the category.</param>
        /// <param name="discountId">The ID of the discount to apply.</param>
        /// <returns>A success message if the discount is applied.</returns>
        [HttpPost("apply-to-category/{categoryName}/{discountId}")]
        [SwaggerOperation(Summary = "Applies a discount to a category")]
        [SwaggerResponse(200, "Discount applied successfully", typeof(string))]
        [SwaggerResponse(400, "Invalid input")]
        [SwaggerResponse(401, "Unauthorized - User is not authenticated")]
        [SwaggerResponse(403, "Forbidden - User does not have permission")]
        [SwaggerResponse(404, "Category or discount not found")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult<string>> ApplyDiscountToCategory([Required] string categoryName, [Required] int discountId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(categoryName))
                {
                    return BadRequest(new { message = "Category name cannot be empty." });
                }

                if (discountId <= 0)
                {
                    return BadRequest(new { message = "Discount ID must be a valid positive integer." });
                }

                var response = await _discountService.ApplyDiscountToCategory(categoryName, discountId);
                return Ok(response);
            }
            catch (ServiceException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", error = ex.Message });
            }
        }


        /// <summary>
        /// Applies a discount to all products of a specific brand.
        /// </summary>
        /// <param name="brandName">The name of the brand.</param>
        /// <param name="discountId">The ID of the discount to apply.</param>
        /// <returns>A success message if the discount is applied.</returns>
        [HttpPost("apply-to-brand/{brandName}/{discountId}")]
        [SwaggerOperation(Summary = "Applies a discount to a brand")]
        [SwaggerResponse(200, "Discount applied successfully", typeof(string))]
        [SwaggerResponse(400, "Invalid input")]
        [SwaggerResponse(401, "Unauthorized - User is not authenticated")]
        [SwaggerResponse(403, "Forbidden - User does not have permission")]
        [SwaggerResponse(404, "Brand or discount not found")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult<string>> ApplyDiscountToBrand([Required] string brandName, [Required] int discountId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(brandName))
                {
                    return BadRequest(new { message = "Brand name cannot be empty." });
                }

                if (discountId <= 0)
                {
                    return BadRequest(new { message = "Discount ID must be a valid positive integer." });
                }

                var response = await _discountService.ApplyDiscountToBrand(brandName, discountId);
                return Ok(response);
            }
            catch (ServiceException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", error = ex.Message });
            }
        }


        /// <summary>
        /// Removes all expired discounts from products.
        /// </summary>
        /// <returns>A success message indicating the number of removed discounts.</returns>
        [HttpPost("remove-expired")]
        [SwaggerOperation(Summary = "Removes expired discounts")]
        [SwaggerResponse(200, "Expired discounts removed successfully")]
        [SwaggerResponse(401, "Unauthorized - User is not authenticated")]
        [SwaggerResponse(403, "Forbidden - User does not have permission")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult> RemoveExpiredDiscounts()
        {
            try
            {
                await _discountService.RemoveExpiredDiscounts();
                return Ok(new { message = "Expired discounts removed successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while removing expired discounts.", error = ex.Message });
            }
        }

    }
}
