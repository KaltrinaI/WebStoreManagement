using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using WebStoreApp.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using WebStoreApp.Exceptions;

namespace WebStoreApp.Controllers
{
    /// <summary>
    /// Manages size-related operations.
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/sizes")]
    [ApiVersion("1.0")]
    public class SizeController : ControllerBase
    {
        private readonly ISizeService _sizeService;

        public SizeController(ISizeService sizeService)
        {
            _sizeService = sizeService;
        }

        /// <summary>
        /// Retrieves a size by ID.
        /// </summary>
        [HttpGet("{sizeId}")]
        [SwaggerOperation(Summary = "Gets a size by ID")]
        [SwaggerResponse(200, "Success", typeof(string))]
        [SwaggerResponse(404, "Size not found")]
        public async Task<ActionResult<string>> GetSizeById(int sizeId)
        {
            try
            {
                var response = await _sizeService.GetSizeById(sizeId);
                return Ok(response);
            }
            catch (ServiceException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves a size by name.
        /// </summary>
        [HttpGet("name/{sizeName}")]
        [SwaggerOperation(Summary = "Gets a size by name")]
        [SwaggerResponse(200, "Success", typeof(string))]
        [SwaggerResponse(404, "Size not found")]
        public async Task<ActionResult<string>> GetSizeByName(string sizeName)
        {
            try
            {
                var response = await _sizeService.GetSizeByName(sizeName);
                return Ok(new { message = "Size retrieved successfully.", size = response });
            }
            catch (ServiceException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves all sizes.
        /// </summary>
        [HttpGet]
        [SwaggerOperation(Summary = "Gets all sizes")]
        [SwaggerResponse(200, "Success", typeof(IEnumerable<string>))]
        public async Task<ActionResult<IEnumerable<string>>> GetAllSizes()
        {
            var response = await _sizeService.GetAllSizes();
            return Ok(response );
        }

        /// <summary>
        /// Adds a new size.
        /// </summary>
        [HttpPost]
        [SwaggerOperation(Summary = "Adds a new size")]
        [SwaggerResponse(201, "Size added successfully")]
        [SwaggerResponse(400, "Invalid input")]
        public async Task<ActionResult<string>> AddSize([FromBody] string sizeName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(sizeName))
                {
                    return BadRequest(new { message = "Size name cannot be empty." });
                }

                var response = await _sizeService.AddSize(sizeName);
                return CreatedAtAction(nameof(GetSizeByName), new { sizeName }, new { message = "Size added successfully.", size = response });
            }
            catch (ServiceException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Updates an existing size.
        /// </summary>
        [HttpPut("{sizeId}")]
        [SwaggerOperation(Summary = "Updates an existing size")]
        [SwaggerResponse(200, "Size updated successfully")]
        [SwaggerResponse(400, "Invalid input")]
        [SwaggerResponse(404, "Size not found")]
        public async Task<ActionResult<string>> UpdateSize(int sizeId, [FromBody] string sizeName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(sizeName))
                {
                    return BadRequest(new { message = "Size name cannot be empty." });
                }

                var response = await _sizeService.UpdateSize(sizeId, sizeName);
                return Ok(new { message = "Size updated successfully.", size = response });
            }
            catch (ServiceException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Deletes a size by ID.
        /// </summary>
        [HttpDelete("{sizeId}")]
        [SwaggerOperation(Summary = "Deletes a size by ID")]
        [SwaggerResponse(200, "Size deleted successfully")]
        [SwaggerResponse(404, "Size not found")]
        public async Task<ActionResult<string>> DeleteSize(int sizeId)
        {
            try
            {
                var response = await _sizeService.DeleteSize(sizeId);
                return Ok(new { message = "Size deleted successfully.", size = response });
            }
            catch (ServiceException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
