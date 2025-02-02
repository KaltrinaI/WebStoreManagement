using Microsoft.EntityFrameworkCore;
using WebStoreApp.Data;
using WebStoreApp.Models;
using WebStoreApp.Repositories.Interfaces;
using WebStoreApp.Exceptions; 
using Microsoft.Extensions.Logging;

namespace WebStoreApp.Repositories.Implementations
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CategoryRepository> _logger;

        public CategoryRepository(AppDbContext context, ILogger<CategoryRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task AddCategory(string CategoryName)
        {
            try
            {
                Category requestBody = new Category { Name = CategoryName };
                _context.Categories.Add(requestBody);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error occurred while adding a category: {CategoryName}", CategoryName);
                throw new RepositoryException("Error occurred while adding a category.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while adding a category: {CategoryName}", CategoryName);
                throw new RepositoryException("An unexpected error occurred while adding a category.", ex);
            }
        }

        public async Task DeleteCategory(int CategoryId)
        {
            try
            {
                var category = await _context.Categories.FindAsync(CategoryId);
                if (category == null)
                {
                    _logger.LogWarning("Category with ID {CategoryId} not found.", CategoryId);
                    throw new RepositoryException($"Category with ID {CategoryId} not found.");
                }

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error occurred while deleting a category with ID: {CategoryId}", CategoryId);
                throw new RepositoryException("Error occurred while deleting a category.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while deleting a category with ID: {CategoryId}", CategoryId);
                throw new RepositoryException("An unexpected error occurred while deleting a category.", ex);
            }
        }

        public async Task<IEnumerable<Category>> GetAllCategories()
        {
            try
            {
                return await _context.Categories.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all categories.");
                throw new RepositoryException("Error occurred while retrieving all categories.", ex);
            }
        }

        public async Task<Category> GetCategoryById(int CategoryId)
        {
            try
            {
                var category = await _context.Categories.FindAsync(CategoryId);
                if (category == null)
                {
                    _logger.LogWarning("Category with ID {CategoryId} not found.", CategoryId);
                    throw new RepositoryException($"Category with ID {CategoryId} not found.");
                }

                return category;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving the category by ID: {CategoryId}", CategoryId);
                throw new RepositoryException("Error occurred while retrieving the category by ID.", ex);
            }
        }

        public async Task<Category> GetCategoryByName(string Name)
        {
            try
            {
                var category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == Name);
                if (category == null)
                {
                    _logger.LogWarning("Category with name '{Name}' not found.", Name);
                    throw new RepositoryException($"Category with name '{Name}' not found.");
                }

                return category;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving the category by name: {Name}", Name);
                throw new RepositoryException("Error occurred while retrieving the category by name.", ex);
            }
        }

        public async Task UpdateCategory(Category category)
        {
            try
            {
                _context.Entry(category).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency conflict occurred while updating the category with ID: {CategoryId}", category.Id);
                throw new RepositoryException("The category may have been modified by another process.", ex);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error occurred while updating the category with ID: {CategoryId}", category.Id);
                throw new RepositoryException("Error occurred while updating the category.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while updating the category with ID: {CategoryId}", category.Id);
                throw new RepositoryException("An unexpected error occurred while updating the category.", ex);
            }
        }
    }
}
