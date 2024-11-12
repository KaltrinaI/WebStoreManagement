using Microsoft.EntityFrameworkCore;
using WebStoreApp.Data;
using WebStoreApp.Models;
using WebStoreApp.Repositories.Interfaces;

namespace WebStoreApp.Repositories.Implementations
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;
        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddCategory(string CategoryName)
        {
            Category requestBody = new Category();
            requestBody.Name = CategoryName;
            _context.Categories.Add(requestBody);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCategory(int CategoryId)
        {
            var category = await _context.Categories.FindAsync(CategoryId);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Category>> GetAllCategories()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<Category> GetCategoryById(int CategoryId)
        {
            var category = await _context.Categories.FindAsync(CategoryId);
            return category ?? new Category();
        }

        public async Task<Category> GetCategoryByName(string Name)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == Name);
            return category ?? new Category();
        }

        public async Task UpdateCategory(Category category)
        {
            _context.Entry(category).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
