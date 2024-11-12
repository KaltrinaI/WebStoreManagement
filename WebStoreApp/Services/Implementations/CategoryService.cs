using WebStoreApp.Models;
using WebStoreApp.Repositories.Interfaces;
using WebStoreApp.Services.Interfaces;

namespace WebStoreApp.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repository;

        public CategoryService(ICategoryRepository repository)
        {
            _repository = repository;
        }
        public async Task AddCategory(string CategoryName)
        {
            await _repository.AddCategory(CategoryName);
        }

        public async Task DeleteCategory(int CategoryId)
        {
            await _repository.DeleteCategory(CategoryId);
        }

        public async Task<IEnumerable<string>> GetAllCategories()
        {
            var categories = await _repository.GetAllCategories();
            return categories.Select(category => category.Name).ToList();

        }

        public async Task<string> GetCategoryById(int CategoryId)
        {
            var category = await _repository.GetCategoryById(CategoryId);
            return category.Name;
        }

        public async Task<string> GetCategoryByName(string Name)
        {
            var category = await _repository.GetCategoryByName(Name);
            return category.Name;
        }

        public async Task UpdateCategory(int CategoryId, string CategoryName)
        {
            var category = await _repository.GetCategoryById(CategoryId);
            if (category != null)
            {
                category.Name = CategoryName;
                await _repository.UpdateCategory(category);
            }
        }
    }
}
