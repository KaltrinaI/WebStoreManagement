using WebStoreApp.Models;
using WebStoreApp.Repositories.Interfaces;
using WebStoreApp.Services.Interfaces;
using WebStoreApp.Exceptions;
using Microsoft.Extensions.Logging;

namespace WebStoreApp.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repository;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(ICategoryRepository repository, ILogger<CategoryService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task AddCategory(string categoryName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(categoryName))
                {
                    _logger.LogWarning("Category name is invalid or empty.");
                    throw new ServiceException("Category name cannot be empty.");
                }

                await _repository.AddCategory(categoryName);
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, "Error occurred while adding a category: {CategoryName}", categoryName);
                throw new ServiceException("An error occurred while adding the category.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while adding a category: {CategoryName}", categoryName);
                throw new ServiceException("An unexpected error occurred while adding the category.", ex);
            }
        }

        public async Task DeleteCategory(int categoryId)
        {
            try
            {
                if (categoryId <= 0)
                {
                    _logger.LogWarning("Invalid category ID: {CategoryId}", categoryId);
                    throw new ServiceException("Invalid category ID.");
                }

                await _repository.DeleteCategory(categoryId);
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, "Error occurred while deleting category with ID: {CategoryId}", categoryId);
                throw new ServiceException("An error occurred while deleting the category.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while deleting category with ID: {CategoryId}", categoryId);
                throw new ServiceException("An unexpected error occurred while deleting the category.", ex);
            }
        }

        public async Task<IEnumerable<string>> GetAllCategories()
        {
            try
            {
                var categories = await _repository.GetAllCategories();
                return categories.Select(category => category.Name).ToList();
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all categories.");
                throw new ServiceException("An error occurred while retrieving all categories.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while retrieving all categories.");
                throw new ServiceException("An unexpected error occurred while retrieving all categories.", ex);
            }
        }

        public async Task<string> GetCategoryById(int categoryId)
        {
            try
            {
                if (categoryId <= 0)
                {
                    _logger.LogWarning("Invalid category ID: {CategoryId}", categoryId);
                    throw new ServiceException("Invalid category ID.");
                }

                var category = await _repository.GetCategoryById(categoryId);
                if (category == null || string.IsNullOrEmpty(category.Name))
                {
                    _logger.LogWarning("Category with ID {CategoryId} not found.", categoryId);
                    throw new ServiceException($"Category with ID {categoryId} not found.");
                }

                return category.Name;
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving category by ID: {CategoryId}", categoryId);
                throw new ServiceException("An error occurred while retrieving the category by ID.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while retrieving category by ID: {CategoryId}", categoryId);
                throw new ServiceException("An unexpected error occurred while retrieving the category by ID.", ex);
            }
        }

        public async Task<string> GetCategoryByName(string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    _logger.LogWarning("Category name is invalid or empty.");
                    throw new ServiceException("Category name cannot be empty.");
                }

                var category = await _repository.GetCategoryByName(name);
                if (category == null || string.IsNullOrEmpty(category.Name))
                {
                    _logger.LogWarning("Category with name '{CategoryName}' not found.", name);
                    throw new ServiceException($"Category with name '{name}' not found.");
                }

                return category.Name;
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving category by name: {CategoryName}", name);
                throw new ServiceException("An error occurred while retrieving the category by name.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while retrieving category by name: {CategoryName}", name);
                throw new ServiceException("An unexpected error occurred while retrieving the category by name.", ex);
            }
        }

        public async Task UpdateCategory(int categoryId, string categoryName)
        {
            try
            {
                if (categoryId <= 0)
                {
                    _logger.LogWarning("Invalid category ID: {CategoryId}", categoryId);
                    throw new ServiceException("Invalid category ID.");
                }

                if (string.IsNullOrWhiteSpace(categoryName))
                {
                    _logger.LogWarning("Category name is invalid or empty.");
                    throw new ServiceException("Category name cannot be empty.");
                }

                var category = await _repository.GetCategoryById(categoryId);
                if (category == null || string.IsNullOrEmpty(category.Name))
                {
                    _logger.LogWarning("Category with ID {CategoryId} not found.", categoryId);
                    throw new ServiceException($"Category with ID {categoryId} not found.");
                }

                category.Name = categoryName;
                await _repository.UpdateCategory(category);
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, "Error occurred while updating category with ID {CategoryId}.", categoryId);
                throw new ServiceException("An error occurred while updating the category.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while updating category with ID {CategoryId}.", categoryId);
                throw new ServiceException("An unexpected error occurred while updating the category.", ex);
            }
        }
    }
}
