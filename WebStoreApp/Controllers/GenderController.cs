using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using WebStoreApp.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using WebStoreApp.Exceptions;

namespace WebStoreApp.Controllers
{
    /// <summary>
    /// Manages gender operations.
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/genders")]
    [ApiVersion("1.0")]
    public class GenderController : ControllerBase
    {
        private readonly IGenderService _service;

        public GenderController(IGenderService service)
        {
            _service = service;
        }

        /// <summary>
        /// Retrieves a gender by its unique identifier.
        /// </summary>
        /// <param name="GenderId">The ID of the gender.</param>
        /// <returns>The gender name if found; otherwise, a `404 Not Found` response.</returns>
        [HttpGet("id/{GenderId}")]
        [SwaggerOperation(Summary = "Gets a gender by ID")]
        [SwaggerResponse(200, "Success", typeof(string))]
        [SwaggerResponse(400, "Invalid input")]
        [SwaggerResponse(404, "Gender not found")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult<string>> GetGenderById([Required] int GenderId)
        {
            try
            {
                if (GenderId <= 0)
                {
                    return BadRequest(new { message = "Gender ID must be a valid positive integer." });
                }

                var response = await _service.GetGenderById(GenderId);
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
        /// Retrieves a gender by its name.
        /// </summary>
        /// <param name="GenderName">The name of the gender.</param>
        /// <returns>The gender name if found; otherwise, a `404 Not Found` response.</returns>
        [HttpGet("name/{GenderName}")]
        [SwaggerOperation(Summary = "Gets a gender by name")]
        [SwaggerResponse(200, "Success", typeof(string))]
        [SwaggerResponse(400, "Invalid input")]
        [SwaggerResponse(404, "Gender not found")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult<string>> GetGenderByName([Required] string GenderName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(GenderName))
                {
                    return BadRequest(new { message = "Gender name cannot be empty." });
                }

                var response = await _service.GetGenderByName(GenderName);
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
        /// Retrieves all available genders.
        /// </summary>
        /// <returns>A list of all genders.</returns>
        [HttpGet]
        [SwaggerOperation(Summary = "Gets all genders")]
        [SwaggerResponse(200, "Success", typeof(IEnumerable<string>))]
        [SwaggerResponse(404, "No genders found")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult<IEnumerable<string>>> GetAllGenders()
        {
            try
            {
                var response = await _service.GetAllGenders();

                if (!response.Any())
                {
                    return NotFound(new { message = "No genders found." });
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred while retrieving genders.", error = ex.Message });
            }
        }


        /// <summary>
        /// Adds a new gender.
        /// </summary>
        /// <param name="GenderName">The name of the gender to add.</param>
        /// <returns>A success message if the gender is added.</returns>
        [HttpPost]
        [SwaggerOperation(Summary = "Adds a new gender")]
        [SwaggerResponse(201, "Gender created successfully")]
        [SwaggerResponse(400, "Invalid input")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult> AddGender([FromBody, Required] string GenderName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(GenderName))
                {
                    return BadRequest(new { message = "Gender name cannot be empty." });
                }

                await _service.AddGender(GenderName);

                return StatusCode(201, new { message = "Gender created successfully." });
            }
            catch (ServiceException ex)
            {
                return BadRequest(new { message = "Error adding gender.", error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred while adding the gender.", error = ex.Message });
            }
        }


        /// <summary>
        /// Updates an existing gender.
        /// </summary>
        /// <param name="GenderId">The ID of the gender to update.</param>
        /// <param name="GenderName">The new name of the gender.</param>
        /// <returns>A success message if the gender is updated.</returns>
        [HttpPut("{GenderId}")]
        [SwaggerOperation(Summary = "Updates an existing gender")]
        [SwaggerResponse(200, "Gender updated successfully")]
        [SwaggerResponse(400, "Invalid input")]
        [SwaggerResponse(404, "Gender not found")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult> UpdateGender([Required] int GenderId, [FromBody, Required] string GenderName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(GenderName))
                {
                    return BadRequest(new { message = "Gender name cannot be empty." });
                }

                await _service.UpdateGender(GenderId, GenderName);
                return Ok(new { message = $"Gender with ID {GenderId} updated successfully." });
            }
            catch (ServiceException ex)
            {
                return NotFound(new { message = $"Gender with ID {GenderId} not found.", error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred while updating the gender.", error = ex.Message });
            }
        }


        /// <summary>
        /// Deletes a gender by its ID.
        /// </summary>
        /// <param name="GenderId">The ID of the gender to delete.</param>
        /// <returns>A success message if the gender is deleted.</returns>
        [HttpDelete("{GenderId}")]
        [SwaggerOperation(Summary = "Deletes a gender by ID")]
        [SwaggerResponse(204, "Gender deleted successfully")]
        [SwaggerResponse(404, "Gender not found")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult> DeleteGender([Required] int GenderId)
        {
            try
            {
                await _service.DeleteGender(GenderId);
                return NoContent();
            }
            catch (ServiceException ex)
            {
                return NotFound(new { message = $"Gender with ID {GenderId} not found.", error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred while deleting the gender.", error = ex.Message });
            }
        }

    }
}
