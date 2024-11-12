using Microsoft.AspNetCore.Mvc;
using WebStoreApp.Services.Implementations;
using WebStoreApp.Services.Interfaces;

namespace WebStoreApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SizeController : ControllerBase
    {
        private readonly ISizeService _service;
        public SizeController(ISizeService service)
        {
            _service = service;
        }

        [HttpGet("id/{SizeId}")]
        public async Task<ActionResult<string>> GetSizeById(int SizeId)
        {
            var response = await _service.GetSizeById(SizeId);
            return Ok(response);
        }
        [HttpGet("name/{SizeName}")]
        public async Task<ActionResult<string>> GetSizeByName(string SizeName)
        {
            var response = await _service.GetSizeByName(SizeName);
            return Ok(response);
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> GetAllSizes()
        {
            var response = await _service.GetAllSizes();
            return Ok(response);
        }
        [HttpPost]
        public async Task<ActionResult<string>> AddSize([FromBody] string SizeName)
        {
            var response = await _service.AddSize(SizeName);
            return Ok(response);
        }

        [HttpPut("{SizeId}")]
        public async Task<ActionResult<string>> UpdateSize(int SizeId, [FromBody] string SizeName)
        {
            var response = await _service.UpdateSize(SizeId, SizeName);
            return Ok(response);
        }

        [HttpDelete("{SizeId}")]
        public async Task<ActionResult<string>> DeleteSize(int SizeId)
        {

            var response = await _service.DeleteSize(SizeId);
            return Ok(response);

        }

    }
}
