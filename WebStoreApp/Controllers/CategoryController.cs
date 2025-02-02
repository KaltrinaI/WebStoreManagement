using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using WebStoreApp.Services.Interfaces;
using WebStoreApp.Exceptions; // Importing ServiceException
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebStoreApp.Controllers
{
    /// <summary>
    /// Manages category operations.
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/categories")]
    [ApiVersion("1.0")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _service;

        public CategoryController(ICategoryService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets a category by its ID.
        /// </summary>
        /// <param name="CategoryId">The ID of the category to retrieve.</param>
        [HttpGet("id/{CategoryId}")]
        [SwaggerOperation(Summary = "Gets a category by ID")]
        [SwaggerResponse(200, "Success", typeof(string))]
        [SwaggerResponse(404, "Category not found")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult<string>> GetCategoryById([Required] int CategoryId)
        {
            try
            {
                var response = await _service.GetCategoryById(CategoryId);
                return Ok(response);
            }
            catch (ServiceException ex)
            {
                return NotFound(new { message = $"Category with ID {CategoryId} not found.", error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", error = ex.Message });
            }
        }

        /// <summary>
        /// Gets a category by its name.
        /// </summary>
        /// <param name="CategoryName">The name of the category to retrieve.</param>
        [HttpGet("name/{CategoryName}")]
        [SwaggerOperation(Summary = "Gets a category by name")]
        [SwaggerResponse(200, "Success", typeof(string))]
        [SwaggerResponse(404, "Category not found")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult<string>> GetCategoryByName([Required] string CategoryName)
        {
            try
            {
                var response = await _service.GetCategoryByName(CategoryName);
                return Ok(response);
            }
            catch (ServiceException ex)
            {
                return NotFound(new { message = $"Category with name '{CategoryName}' not found.", error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", error = ex.Message });
            }
        }

        /// <summary>
        /// Gets all categories.
        /// </summary>
        [HttpGet]
        [SwaggerOperation(Summary = "Gets all categories")]
        [SwaggerResponse(200, "Success", typeof(IEnumerable<string>))]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult<IEnumerable<string>>> GetAllCategories()
        {
            try
            {
                var response = await _service.GetAllCategories();
                return Ok(response);
            }
            catch (ServiceException ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving categories.", error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", error = ex.Message });
            }
        }

        /// <summary>
        /// Adds a new category.
        /// </summary>
        /// <param name="CategoryName">The name of the new category.</param>
        [HttpPost]
        [SwaggerOperation(Summary = "Adds a new category")]
        [SwaggerResponse(201, "Category created successfully")]
        [SwaggerResponse(400, "Invalid input")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult> AddCategory([FromBody, Required] string CategoryName)
        {
            try
            {
                await _service.AddCategory(CategoryName);
                return CreatedAtAction(nameof(GetCategoryByName), new { CategoryName }, new { message = $"Category '{CategoryName}' created successfully." });
            }
            catch (ServiceException ex)
            {
                return BadRequest(new { message = $"Error adding category '{CategoryName}'.", error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An unexpected error occurred while adding category '{CategoryName}'.", error = ex.Message });
            }
        }

        /// <summary>
        /// Updates an existing category.
        /// </summary>
        /// <param name="CategoryId">The ID of the category to update.</param>
        /// <param name="CategoryName">The new name for the category.</param>
        [HttpPut("{CategoryId}")]
        [SwaggerOperation(Summary = "Updates an existing category")]
        [SwaggerResponse(200, "Category updated successfully")]
        [SwaggerResponse(400, "Invalid input")]
        [SwaggerResponse(404, "Category not found")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult> UpdateCategory([Required] int CategoryId, [FromBody, Required] string CategoryName)
        {
            try
            {
                await _service.UpdateCategory(CategoryId, CategoryName);
                return Ok(new { message = $"Category with ID {CategoryId} updated successfully." });
            }
            catch (ServiceException ex)
            {
                return NotFound(new { message = $"Category with ID {CategoryId} not found.", error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An unexpected error occurred while updating category with ID {CategoryId}.", error = ex.Message });
            }
        }

        /// <summary>
        /// Deletes a category by its ID.
        /// </summary>
        /// <param name="CategoryId">The ID of the category to delete.</param>
        [HttpDelete("{CategoryId}")]
        [SwaggerOperation(Summary = "Deletes a category by ID")]
        [SwaggerResponse(200, "Category deleted successfully")]
        [SwaggerResponse(404, "Category not found")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult> DeleteCategory([Required] int CategoryId)
        {
            try
            {
                await _service.DeleteCategory(CategoryId);
                return Ok(new { message = $"Category with ID {CategoryId} deleted successfully." });
            }
            catch (ServiceException ex)
            {
                return NotFound(new { message = $"Category with ID {CategoryId} not found.", error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An unexpected error occurred while deleting category with ID {CategoryId}.", error = ex.Message });
            }
        }
    }
}
