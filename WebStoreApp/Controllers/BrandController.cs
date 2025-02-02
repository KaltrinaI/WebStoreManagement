using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using WebStoreApp.Exceptions;
using WebStoreApp.Services.Interfaces;

namespace WebStoreApp.Controllers
{
    /// <summary>
    /// Manages brand operations.
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/brands")]
    [ApiVersion("1.0")]
    public class BrandController : ControllerBase
    {
        private readonly IBrandService _service;

        public BrandController(IBrandService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets a brand by its ID.
        /// </summary>
        /// <param name="BrandId">The ID of the brand.</param>
        /// <returns>The brand name.</returns>
        [HttpGet("id/{BrandId}")]
        [SwaggerOperation(Summary = "Gets a brand by ID")]
        [SwaggerResponse(200, "Success", typeof(string))]
        [SwaggerResponse(404, "Brand not found")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult<string>> GetBrandById([Required] int BrandId)
        {
            try
            {
                var response = await _service.GetBrandById(BrandId);
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
        /// Gets a brand by its name.
        /// </summary>
        /// <param name="BrandName">The name of the brand.</param>
        /// <returns>The brand name.</returns>
        [HttpGet("name/{BrandName}")]
        [SwaggerOperation(Summary = "Gets a brand by name")]
        [SwaggerResponse(200, "Success", typeof(string))]
        [SwaggerResponse(404, "Brand not found")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult<string>> GetBrandByName([Required] string BrandName)
        {
            try
            {
                var response = await _service.GetBrandByName(BrandName);
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
        /// Gets all brands.
        /// </summary>
        /// <returns>A list of brand names.</returns>
        [HttpGet]
        [SwaggerOperation(Summary = "Gets all brands")]
        [SwaggerResponse(200, "Success", typeof(IEnumerable<string>))]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult<IEnumerable<string>>> GetAllBrands()
        {
            try
            {
                var response = await _service.GetAllBrands();
                return Ok(response);
            }
            catch (ServiceException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", error = ex.Message });
            }
        }

        /// <summary>
        /// Adds a new brand.
        /// </summary>
        /// <param name="BrandName">The name of the brand to add.</param>
        /// <returns>Success message.</returns>
        [HttpPost]
        [SwaggerOperation(Summary = "Adds a new brand")]
        [SwaggerResponse(201, "Brand created successfully")]
        [SwaggerResponse(400, "Invalid input")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult> AddBrand([FromBody, Required] string BrandName)
        {
            try
            {
                await _service.AddBrand(BrandName);
                return CreatedAtAction(nameof(GetBrandByName), new { BrandName }, "Brand created successfully.");
            }
            catch (ServiceException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", error = ex.Message });
            }
        }

        /// <summary>
        /// Updates an existing brand.
        /// </summary>
        /// <param name="BrandId">The ID of the brand to update.</param>
        /// <param name="BrandName">The new name of the brand.</param>
        /// <returns>Success message.</returns>
        [HttpPut("{BrandId}")]
        [SwaggerOperation(Summary = "Updates an existing brand")]
        [SwaggerResponse(200, "Brand updated successfully")]
        [SwaggerResponse(400, "Invalid input")]
        [SwaggerResponse(404, "Brand not found")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult> UpdateBrand([Required] int BrandId, [FromBody, Required] string BrandName)
        {
            try
            {
                await _service.UpdateBrand(BrandId, BrandName);
                return Ok("Brand updated successfully.");
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
        /// Deletes a brand by its ID.
        /// </summary>
        /// <param name="BrandId">The ID of the brand to delete.</param>
        /// <returns>Success message.</returns>
        [HttpDelete("{BrandId}")]
        [SwaggerOperation(Summary = "Deletes a brand by ID")]
        [SwaggerResponse(200, "Brand deleted successfully")]
        [SwaggerResponse(404, "Brand not found")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult> DeleteBrand([Required] int BrandId)
        {
            try
            {
                await _service.DeleteBrand(BrandId);
                return Ok("Brand deleted successfully.");
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
    }
}
