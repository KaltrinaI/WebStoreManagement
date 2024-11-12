using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebStoreApp.DTOs;
using WebStoreApp.Services.Interfaces;

namespace WebStoreApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }


        [HttpGet]
        [Authorize(Roles = "Admin,AdvancedUser")]
        public async Task<ActionResult<IEnumerable<ReportDTO>>> GetAllReports()
        {
            var reports = await _reportService.GetAllReports();
            return Ok(reports);
        }

        [HttpGet("earnings/daily")]
        [Authorize(Roles = "Admin,AdvancedUser")]
        public async Task<ActionResult<double>> GetDailyEarnings()
        {
            var earnings = await _reportService.GetDailyEarnings();
      
            return Ok(earnings);
        }

        [HttpGet("earnings/monthly")]
        [Authorize(Roles = "Admin,AdvancedUser")]
        public async Task<ActionResult<double>> GetMonthlyEarnings([FromQuery] int month, [FromQuery] int year)
        {
            var earnings = await _reportService.GetMonthlyEarnings(month, year);
            return Ok(earnings);
        }

        [HttpGet("earnings/total")]
        [Authorize(Roles = "Admin,AdvancedUser")]
        public async Task<ActionResult<double>> GetTotalEarnings()
        {
            var earnings = await _reportService.GetTotalEarnings();
            return Ok(earnings);
        }
    }
}
