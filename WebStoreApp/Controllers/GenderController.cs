using Microsoft.AspNetCore.Mvc;
using WebStoreApp.Services.Interfaces;

namespace WebStoreApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenderController : ControllerBase
    {
        private readonly IGenderService _service;
        public GenderController(IGenderService service)
        {
            _service = service;
        }

        [HttpGet("id/{GenderId}")]
        public async Task<ActionResult<string>> GetGenderById(int GenderId)
        {
            var response = await _service.GetGenderById(GenderId);
            return Ok(response);
        }
        [HttpGet("name/{GenderName}")]
        public async Task<ActionResult<string>> GetGenderByName(string GenderName)
        {
            var response = await _service.GetGenderByName(GenderName);
            return Ok(response);
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> GetAllGenders()
        {
            var response = await _service.GetAllGenders();
            return Ok(response);
        }
        [HttpPost]
        public async Task<ActionResult> AddGender([FromBody] string GenderName)
        {
            await _service.AddGender(GenderName);
            return Ok("Gender Created Successfully");
        }

        [HttpPut("{GenderId}")]
        public async Task<ActionResult> UpdateGender(int GenderId, [FromBody] string GenderName)
        {
            await _service.UpdateGender(GenderId, GenderName);
            return Ok("Gender Updated Succesfully");
        }
        [HttpDelete("{GenderId}")]
        public async Task<ActionResult> DeleteGender(int GenderId)
        {
            await _service.DeleteGender(GenderId);
            return Ok("Gender Deleted Successfully");
        }
    }
}

