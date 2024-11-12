using WebStoreApp.Models;

namespace WebStoreApp.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<string> GetCategoryById(int CategoryId);
        Task<string> GetCategoryByName(string Name);
        Task<IEnumerable<string>> GetAllCategories();
        Task AddCategory(string CategoryName);
        Task UpdateCategory(int CategoryId, string Category);
        Task DeleteCategory(int CategoryId);
    }
}
