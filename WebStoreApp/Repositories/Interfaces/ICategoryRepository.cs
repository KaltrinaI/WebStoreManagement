using WebStoreApp.Models;

namespace WebStoreApp.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<Category> GetCategoryById(int CategoryId);
        Task<Category> GetCategoryByName(string Name);
        Task<IEnumerable<Category>> GetAllCategories();
        Task AddCategory(string CategoryName);
        Task UpdateCategory(Category category);
        Task DeleteCategory(int CategoryId);
    }
}
