using Microsoft.EntityFrameworkCore;
using Npgsql.Internal;
using System.Drawing;
using System.Reflection;
using WebStoreApp.Data;
using WebStoreApp.Models;
using WebStoreApp.Repositories.Interfaces;

namespace WebStoreApp.Repositories.Implementations
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddProduct(Product Product)
        {
            await _context.Products.AddAsync(Product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProduct(int ProductId)
        {
            var product = await _context.Products.FindAsync(ProductId);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Product>> GetAllProducts()
        {
            return await _context.Products
             .Include(p => p.Category)
             .Include(p => p.Brand)
             .Include(p => p.Color)
             .Include(p => p.Gender)
             .Include(p => p.Size)
             .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetOutOfStockProducts()
        {
            return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.Gender)
            .Include(p => p.Color)
            .Include(p => p.Size)
            .Where(p => p.Quantity <= 0)
            .ToListAsync();
        }

        public async Task<Product> GetProductById(int Id)
        {
            var product = await _context.Products
             .Include(p => p.Category)
             .Include(p => p.Brand)
             .Include(p => p.Color)
             .Include(p => p.Gender)
             .Include(p => p.Size)
             .FirstOrDefaultAsync(p => p.Id == Id);

            return product ?? new Product();
        }

        public async Task<IEnumerable<Product>> GetProductsByBrand(string Brand)
        {
            return await _context.Products
               .Include(p => p.Category)
               .Include(p => p.Brand)
               .Include(p => p.Gender)
               .Include(p => p.Color)
               .Include(p => p.Size)
               .Where(p => p.Brand.Name.ToLower() == Brand.ToLower())
               .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByCategory(string Category)
        {
            return await _context.Products
               .Include(p => p.Category)
               .Include(p => p.Brand)
               .Include(p => p.Gender)
               .Include(p => p.Color)
               .Include(p => p.Size)
               .Where(p => p.Category.Name.ToLower() == Category.ToLower()) 
               .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByColor(string Color)
        {
            return await _context.Products
               .Include(p => p.Category)
               .Include(p => p.Brand)
               .Include(p => p.Gender)
               .Include(p => p.Color)
               .Include(p => p.Size)
               .Where(p => p.Color.Name.ToLower() == Color.ToLower())
               .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByGender(string Gender)
        {
            return await _context.Products
               .Include(p => p.Category)
               .Include(p => p.Brand)
               .Include(p => p.Gender)
               .Include(p => p.Color)
               .Include(p => p.Size)
               .Where(p => p.Gender.Name.ToLower() == Gender.ToLower())
               .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsBySize(string Size)
        {
            return await _context.Products
               .Include(p => p.Category)
               .Include(p => p.Brand)
               .Include(p => p.Gender)
               .Include(p => p.Color)
               .Include(p => p.Size)
               .Where(p => p.Size.Name.ToLower() == Size.ToLower())
               .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsWithDiscount()
        {
            return await _context.Products
             .Include(p => p.Category)
             .Include(p => p.Brand)
             .Include(p => p.Color)
             .Include(p => p.Gender)
             .Include(p => p.Size)
             .Where(p => p.Discounts.Any())
             .ToListAsync();
        }

        public async Task UpdateProduct(int ProductId, Product Product)
        {
            var existingProduct = await GetProductById(ProductId);
            if (existingProduct != null)
            {
                existingProduct.Name = Product.Name;
                existingProduct.Description = Product.Description;
                existingProduct.Price = Product.Price;
                existingProduct.Quantity = Product.Quantity;
                existingProduct.CategoryId = Product.CategoryId;
                existingProduct.BrandId = Product.BrandId;
                existingProduct.ColorId = Product.ColorId;
                existingProduct.GenderId = Product.GenderId;
                existingProduct.SizeId = Product.SizeId;
                _context.Entry(existingProduct).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
        }
    }
}
