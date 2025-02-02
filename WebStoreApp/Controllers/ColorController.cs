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
    /// Manages color operations.
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/colors")]
    [ApiVersion("1.0")]
    public class ColorController : ControllerBase
    {
        private readonly IColorService _service;

        public ColorController(IColorService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets a color by its ID.
        /// </summary>
        /// <param name="ColorId">The ID of the color to retrieve.</param>
        [HttpGet("id/{ColorId}")]
        [SwaggerOperation(Summary = "Gets a color by ID")]
        [SwaggerResponse(200, "Success", typeof(string))]
        [SwaggerResponse(404, "Color not found")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult<string>> GetColorById([Required] int ColorId)
        {
            try
            {
                var response = await _service.GetColorById(ColorId);
                return Ok(response);
            }
            catch (ServiceException ex)
            {
                return NotFound(new { message = $"Color with ID {ColorId} not found.", error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", error = ex.Message });
            }
        }

        /// <summary>
        /// Gets a color by its name.
        /// </summary>
        /// <param name="ColorName">The name of the color to retrieve.</param>
        [HttpGet("name/{ColorName}")]
        [SwaggerOperation(Summary = "Gets a color by name")]
        [SwaggerResponse(200, "Success", typeof(string))]
        [SwaggerResponse(404, "Color not found")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult<string>> GetColorByName([Required] string ColorName)
        {
            try
            {
                var response = await _service.GetColorByName(ColorName);
                return Ok(response);
            }
            catch (ServiceException ex)
            {
                return NotFound(new { message = $"Color with name '{ColorName}' not found.", error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", error = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves all colors.
        /// </summary>
        [HttpGet]
        [SwaggerOperation(Summary = "Gets all colors")]
        [SwaggerResponse(200, "Success", typeof(IEnumerable<string>))]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult<IEnumerable<string>>> GetAllColors()
        {
            try
            {
                var response = await _service.GetAllColors();
                return Ok(response);
            }
            catch (ServiceException ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving colors.", error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", error = ex.Message });
            }
        }

        /// <summary>
        /// Adds a new color.
        /// </summary>
        /// <param name="ColorName">The name of the new color.</param>
        [HttpPost]
        [SwaggerOperation(Summary = "Adds a new color")]
        [SwaggerResponse(201, "Color created successfully")]
        [SwaggerResponse(400, "Invalid input")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult> AddColor([FromBody, Required] string ColorName)
        {
            try
            {
                await _service.AddColor(ColorName);
                return CreatedAtAction(nameof(GetColorByName), new { ColorName }, new { message = $"Color '{ColorName}' created successfully." });
            }
            catch (ServiceException ex)
            {
                return BadRequest(new { message = $"Error adding color '{ColorName}'.", error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An unexpected error occurred while adding color '{ColorName}'.", error = ex.Message });
            }
        }

        /// <summary>
        /// Updates an existing color.
        /// </summary>
        /// <param name="ColorId">The ID of the color to update.</param>
        /// <param name="ColorName">The new name for the color.</param>
        [HttpPut("{ColorId}")]
        [SwaggerOperation(Summary = "Updates an existing color")]
        [SwaggerResponse(200, "Color updated successfully")]
        [SwaggerResponse(400, "Invalid input")]
        [SwaggerResponse(404, "Color not found")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult> UpdateColor([Required] int ColorId, [FromBody, Required] string ColorName)
        {
            try
            {
                await _service.UpdateColor(ColorId, ColorName);
                return Ok(new { message = $"Color with ID {ColorId} updated successfully." });
            }
            catch (ServiceException ex)
            {
                return NotFound(new { message = $"Color with ID {ColorId} not found.", error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An unexpected error occurred while updating color with ID {ColorId}.", error = ex.Message });
            }
        }

        /// <summary>
        /// Deletes a color by its ID.
        /// </summary>
        /// <param name="ColorId">The ID of the color to delete.</param>
        [HttpDelete("{ColorId}")]
        [SwaggerOperation(Summary = "Deletes a color by ID")]
        [SwaggerResponse(204, "Color deleted successfully.")]
        [SwaggerResponse(404, "Color not found.")]
        [SwaggerResponse(500, "Internal server error.")]
        public async Task<IActionResult> DeleteColor([Required] int ColorId)
        {
            try
            {
                await _service.DeleteColor(ColorId);
                return NoContent();
            }
            catch (ServiceException ex)
            {
                return NotFound(new { message = $"Color with ID {ColorId} not found.", error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An unexpected error occurred while deleting color with ID {ColorId}.", error = ex.Message });
            }
        }
    }
}
