using Microsoft.AspNetCore.Mvc;
using WebStoreApp.Services.Interfaces;

namespace WebStoreApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ColorController : ControllerBase
    {
        private readonly IColorService _service;
        public ColorController(IColorService service)
        {
            _service = service;
        }

        [HttpGet("id/{ColorId}")]
        public async Task<ActionResult<string>> GetColorById(int ColorId)
        {
            var response = await _service.GetColorById(ColorId);
            return Ok(response);
        }
        [HttpGet("name/{ColorName}")]
        public async Task<ActionResult<string>> GetColorByName(string ColorName)
        {
            var response = await _service.GetColorByName(ColorName);
            return Ok(response);
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> GetAllColors()
        {
            var response = await _service.GetAllColors();
            return Ok(response);
        }
        [HttpPost]
        public async Task<ActionResult> AddColor([FromBody] string ColorName)
        {
            await _service.AddColor(ColorName);
            return Ok("Color Created Successfully");
        }

        [HttpPut("{ColorId}")]
        public async Task<ActionResult> UpdateColor(int ColorId, [FromBody] string ColorName)
        {
            await _service.UpdateColor(ColorId, ColorName);
            return Ok("Color Updated Succesfully");
        }
        [HttpDelete("{ColorId}")]
        public async Task<ActionResult> DeleteColor(int ColorId)
        {
            await _service.DeleteColor(ColorId);
            return Ok("Color Deleted Successfully");
        }
    }
}
