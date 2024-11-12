using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebStoreApp.DTOs;
using WebStoreApp.Services.Interfaces;

namespace WebStoreApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountController : ControllerBase
    {
        private readonly IDiscountService _discountService;

        public DiscountController(IDiscountService discountService)
        {
            _discountService = discountService;
        }

        [HttpGet("{discountId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<DiscountDTO>> GetDiscountById(int discountId)
        {
            var discount = await _discountService.GetDiscountById(discountId);
            if (discount == null)
            {
                return NotFound("Discount not found.");
            }
            return Ok(discount);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<DiscountDTO>>> GetAllDiscounts()
        {
            var discounts = await _discountService.GetAllDiscounts();
            return Ok(discounts);
        }

        [HttpGet("by-name/{name}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<DiscountDTO>>> GetDiscountsByName(string name)
        {
            var discounts = await _discountService.GetDiscountsByName(name);
            return Ok(discounts);
        }

        [HttpGet("by-date-range")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<DiscountDTO>>> GetDiscountsByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var discounts = await _discountService.GetDiscountsByDateRange(startDate, endDate);
            return Ok(discounts);
        }

        [HttpGet("by-starting-date")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<DiscountDTO>>> GetDiscountsByStartingDate([FromQuery] DateTime startDate)
        {
            var discounts = await _discountService.GetDiscountsByStartingDate(startDate);
            return Ok(discounts);
        }

        [HttpGet("by-ending-date")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<DiscountDTO>>> GetDiscountsByEndingDate([FromQuery] DateTime endDate)
        {
            var discounts = await _discountService.GetDiscountsByEndingDate(endDate);
            return Ok(discounts);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AddDiscount([FromBody] DiscountDTO discountDto)
        {
            await _discountService.AddDiscount(discountDto);
            return Ok();
        }

        [HttpPut("{discountId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UpdateDiscount(int discountId, [FromBody] DiscountDTO discountDto)
        {
            await _discountService.UpdateDiscount(discountId, discountDto);
            return NoContent();
        }

        [HttpDelete("{discountId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteDiscount(int discountId)
        {
            await _discountService.DeleteDiscount(discountId);
            return NoContent();
        }

        [HttpPost("apply-to-product/{productId}/{discountId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<string>> ApplyDiscountToProduct(int productId, int discountId)
        {
            var response = await _discountService.ApplyDiscountToProduct(productId, discountId);
            return Ok(response);
        }

        [HttpPost("apply-to-category/{categoryName}/{discountId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<string>> ApplyDiscountToCategory(string categoryName, int discountId)
        {
            var response = await _discountService.ApplyDiscountToCategory(categoryName, discountId);
            return Ok(response);
        }

        [HttpPost("apply-to-brand/{brandName}/{discountId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<string>> ApplyDiscountToBrand(string brandName, int discountId)
        {
            var response = await _discountService.ApplyDiscountToBrand(brandName, discountId);
            return Ok(response);
        }

        [HttpPost("remove-expired")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> RemoveExpiredDiscounts()
        {
            await _discountService.RemoveExpiredDiscounts();
            return NoContent();
        }
    }
}
