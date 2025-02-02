using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using WebStoreApp.DTOs;
using WebStoreApp.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebStoreApp.Controllers
{
    /// <summary>
    /// Manages reports and earnings calculations.
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/reports")]
    [ApiVersion("1.0")]
    [Authorize(Roles = "Admin,AdvancedUser")]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        /// <summary>
        /// Retrieves all reports.
        /// </summary>
        [HttpGet]
        [SwaggerOperation(Summary = "Gets all reports")]
        [SwaggerResponse(200, "Success", typeof(IEnumerable<ReportDTO>))]
        [SwaggerResponse(401, "Unauthorized - User is not authenticated")]
        [SwaggerResponse(403, "Forbidden - User does not have permission")]
        public async Task<ActionResult<IEnumerable<ReportDTO>>> GetAllReports()
        {
            var reports = await _reportService.GetAllReports();
            return Ok(reports);
        }

        /// <summary>
        /// Retrieves the earnings for today.
        /// </summary>
        [HttpGet("earnings/daily")]
        [SwaggerOperation(Summary = "Gets daily earnings")]
        [SwaggerResponse(200, "Success", typeof(double))]
        [SwaggerResponse(401, "Unauthorized - User is not authenticated")]
        [SwaggerResponse(403, "Forbidden - User does not have permission")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult<double>> GetDailyEarnings()
        {
            try
            {
                var earnings = await _reportService.GetDailyEarnings();
                return Ok(new { message = "Daily earnings retrieved successfully.", earnings });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving daily earnings.", error = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves the earnings for a specific month.
        /// </summary>
        /// <param name="month">The month (1-12).</param>
        /// <param name="year">The year.</param>
        [HttpGet("earnings/monthly")]
        [SwaggerOperation(Summary = "Gets monthly earnings")]
        [SwaggerResponse(200, "Success", typeof(double))]
        [SwaggerResponse(400, "Invalid input")]
        [SwaggerResponse(401, "Unauthorized - User is not authenticated")]
        [SwaggerResponse(403, "Forbidden - User does not have permission")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult<double>> GetMonthlyEarnings([FromQuery] int month, [FromQuery] int year)
        {
            try
            {
                if (month < 1 || month > 12)
                {
                    return BadRequest(new { message = "Invalid month. Please provide a value between 1 and 12." });
                }

                if (year < 1)
                {
                    return BadRequest(new { message = "Invalid year. Please provide a positive year value." });
                }

                var earnings = await _reportService.GetMonthlyEarnings(month, year);
                return Ok(new { message = "Monthly earnings retrieved successfully.", earnings });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving monthly earnings.", error = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves the total earnings.
        /// </summary>
        [HttpGet("earnings/total")]
        [SwaggerOperation(Summary = "Gets total earnings")]
        [SwaggerResponse(200, "Success", typeof(double))]
        [SwaggerResponse(401, "Unauthorized - User is not authenticated")]
        [SwaggerResponse(403, "Forbidden - User does not have permission")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult<double>> GetTotalEarnings()
        {
            try
            {
                var earnings = await _reportService.GetTotalEarnings();
                return Ok(new { message = "Total earnings retrieved successfully.", earnings });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving total earnings.", error = ex.Message });
            }
        }
    }
}
