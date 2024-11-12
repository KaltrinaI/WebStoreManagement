using Microsoft.AspNetCore.Mvc;
using WebStoreApp.Services.Interfaces;

namespace WebStoreApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _service;
        public CategoryController(ICategoryService service)
        {
            _service = service;
        }

        [HttpGet("id/{CategoryId}")]
        public async Task<ActionResult<string>> GetCategoryById(int CategoryId)
        {
            var response = await _service.GetCategoryById(CategoryId);
            return Ok(response);
        }
        [HttpGet("name/{CategoryName}")]
        public async Task<ActionResult<string>> GetCategoryByName(string CategoryName)
        {
            var response = await _service.GetCategoryByName(CategoryName);
            return Ok(response);
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> GetAllCategories()
        {
            var response = await _service.GetAllCategories();
            return Ok(response);
        }
        [HttpPost]
        public async Task<ActionResult> AddCategory([FromBody] string CategoryName)
        {
            await _service.AddCategory(CategoryName);
            return Ok("Category Created Successfully");
        }

        [HttpPut("{CategoryId}")]
        public async Task<ActionResult> UpdateCategory(int CategoryId, [FromBody] string CategoryName)
        {
            await _service.UpdateCategory(CategoryId, CategoryName);
            return Ok("Category Updated Succesfully");
        }
        [HttpDelete("{CategoryId}")]
        public async Task<ActionResult> DeleteCategory(int CategoryId)
        {
            await _service.DeleteCategory(CategoryId);
            return Ok("Category Deleted Successfully");
        }

    }
}
