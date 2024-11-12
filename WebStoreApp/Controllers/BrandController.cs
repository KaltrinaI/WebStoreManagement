using Microsoft.AspNetCore.Mvc;
using WebStoreApp.Models;
using WebStoreApp.Services.Interfaces;

namespace WebStoreApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandController : ControllerBase
    {
        private readonly IBrandService _service;
        public BrandController(IBrandService service)
        {
            _service = service;
        }

        [HttpGet("id/{BrandId}")]
        public async Task<ActionResult<string>> GetBrandById(int BrandId)
        {
            var response = await _service.GetBrandById(BrandId);
            return Ok(response);
        }
        [HttpGet("name/{BrandName}")]
        public async Task<ActionResult<string>> GetBrandByName(string BrandName)
        {
            var response = await _service.GetBrandByName(BrandName);
            return Ok(response);
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> GetAllBrands()
        {
            var response = await _service.GetAllBrands();
            return Ok(response);
        }
        [HttpPost]
        public async Task<ActionResult> AddBrand([FromBody] string BrandName)
        {
            await _service.AddBrand(BrandName);
            return Ok("Brand Created Successfully");
        }

        [HttpPut("{BrandId}")]
        public async Task<ActionResult> UpdateBrand(int BrandId,[FromBody] string BrandName)
        {
            await _service.UpdateBrand(BrandId, BrandName);
            return Ok("Brand Updated Succesfully");
        }
        [HttpDelete("{BrandId}")]
        public async Task<ActionResult> DeleteBrand(int BrandId)
        {
            await _service.DeleteBrand(BrandId);
            return Ok("Brand Deleted Successfully");
        }
    }
}
