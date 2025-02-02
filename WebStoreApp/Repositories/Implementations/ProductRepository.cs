using Microsoft.EntityFrameworkCore;
using WebStoreApp.Data;
using WebStoreApp.Models;
using WebStoreApp.Repositories.Interfaces;
using WebStoreApp.Exceptions;
using Microsoft.Extensions.Logging;

namespace WebStoreApp.Repositories.Implementations
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ProductRepository> _logger;

        public ProductRepository(AppDbContext context, ILogger<ProductRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task AddProduct(Product product)
        {
            try
            {
                await _context.Products.AddAsync(product);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error occurred while adding a product: {ProductName}", product.Name);
                throw new RepositoryException("Error occurred while adding a product.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while adding a product: {ProductName}", product.Name);
                throw new RepositoryException("An unexpected error occurred while adding a product.", ex);
            }
        }

        public async Task DeleteProduct(int productId)
        {
            try
            {
                var product = await _context.Products.FindAsync(productId);
                if (product == null)
                {
                    _logger.LogWarning("Product with ID {ProductId} not found.", productId);
                    throw new RepositoryException($"Product with ID {productId} not found.");
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error occurred while deleting a product with ID {ProductId}.", productId);
                throw new RepositoryException("Error occurred while deleting a product.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while deleting a product with ID {ProductId}.", productId);
                throw new RepositoryException("An unexpected error occurred while deleting a product.", ex);
            }
        }

        public async Task<IEnumerable<Product>> GetAllProducts()
        {
            try
            {
                return await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Brand)
                    .Include(p => p.Color)
                    .Include(p => p.Gender)
                    .Include(p => p.Size)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all products.");
                throw new RepositoryException("Error occurred while retrieving all products.", ex);
            }
        }

        public async Task<IEnumerable<Product>> GetOutOfStockProducts()
        {
            try
            {
                return await _context.Products
                    .Where(p => p.Quantity <= 0)
                    .Include(p => p.Category)
                    .Include(p => p.Brand)
                    .Include(p => p.Gender)
                    .Include(p => p.Color)
                    .Include(p => p.Size)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving out-of-stock products.");
                throw new RepositoryException("Error occurred while retrieving out-of-stock products.", ex);
            }
        }

        public async Task<Product> GetProductById(int id)
        {
            try
            {
                var product = await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Brand)
                    .Include(p => p.Color)
                    .Include(p => p.Gender)
                    .Include(p => p.Size)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (product == null)
                {
                    _logger.LogWarning("Product with ID {ProductId} not found.", id);
                    throw new RepositoryException($"Product with ID {id} not found.");
                }

                return product;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving product by ID {ProductId}.", id);
                throw new RepositoryException("Error occurred while retrieving the product by ID.", ex);
            }
        }

        public async Task<IEnumerable<Product>> GetProductsByBrand(string brand)
        {
            try
            {
                return await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Brand)
                    .Include(p => p.Color)  
                    .Include(p => p.Gender)
                    .Include(p=> p.Size)
                    .Where(p => p.Brand.Name.ToLower() == brand.ToLower())
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving products by brand: {BrandName}.", brand);
                throw new RepositoryException("Error occurred while retrieving products by brand.", ex);
            }
        }

        public async Task<IEnumerable<Product>> GetProductsByCategory(string category)
        {
            try
            {
                return await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Brand)
                    .Include(p => p.Gender)
                    .Include(p => p.Color)
                    .Include(p => p.Size)
                    .Where(p => p.Category.Name.ToLower() == category.ToLower())
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving products by category: {CategoryName}.", category);
                throw new RepositoryException("Error occurred while retrieving products by category.", ex);
            }
        }

        public async Task<IEnumerable<Product>> GetProductsByColor(string color)
        {
            try
            {
                return await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Brand)
                    .Include(p => p.Gender)
                    .Include(p => p.Color)
                    .Include(p => p.Size)
                    .Where(p => p.Color.Name.ToLower() == color.ToLower())
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving products by color: {ColorName}.", color);
                throw new RepositoryException("Error occurred while retrieving products by color.", ex);
            }
        }

        public async Task<IEnumerable<Product>> GetProductsByGender(string gender)
        {
            try
            {
                return await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Brand)
                    .Include(p => p.Gender)
                    .Include(p => p.Color)
                    .Include(p => p.Size)
                    .Where(p => p.Gender.Name.ToLower() == gender.ToLower())
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving products by gender: {GenderName}.", gender);
                throw new RepositoryException("Error occurred while retrieving products by gender.", ex);
            }
        }

        public async Task<IEnumerable<Product>> GetProductsBySize(string size)
        {
            try
            {
                return await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Brand)
                    .Include(p => p.Gender)
                    .Include(p => p.Color)
                    .Include(p => p.Size)
                    .Where(p => p.Size.Name.ToLower() == size.ToLower())
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving products by size: {SizeName}.", size);
                throw new RepositoryException("Error occurred while retrieving products by size.", ex);
            }
        }

        public async Task<IEnumerable<Product>> GetProductsWithDiscount()
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving products with discounts.");
                throw new RepositoryException("Error occurred while retrieving products with discounts.", ex);
            }
        }

        public async Task UpdateProduct(int productId, Product product)
        {
            try
            {
                var existingProduct = await GetProductById(productId);
                if (existingProduct == null)
                {
                    throw new RepositoryException($"Product with ID {productId} not found.");
                }

                existingProduct.Name = product.Name;
                existingProduct.Description = product.Description;
                existingProduct.Price = product.Price;
                existingProduct.Quantity = product.Quantity;
                existingProduct.CategoryId = product.CategoryId;
                existingProduct.BrandId = product.BrandId;
                existingProduct.ColorId = product.ColorId;
                existingProduct.GenderId = product.GenderId;
                existingProduct.SizeId = product.SizeId;

                _context.Entry(existingProduct).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error occurred while updating the product with ID {ProductId}.", productId);
                throw new RepositoryException("Error occurred while updating the product.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while updating the product with ID {ProductId}.", productId);
                throw new RepositoryException("An unexpected error occurred while updating the product.", ex);
            }
        }

        public async Task<IEnumerable<Product>> FindByPriceRange(double minPrice, double maxPrice)
        {
            try
            {
                return await _context.Products
                    .Include(p => p.Brand)
                    .Include(p => p.Category)
                    .Include(p => p.Color)
                    .Include(p => p.Size)
                    .Include(p => p.Gender)
                    .Where(p => p.Price >= minPrice && p.Price <= maxPrice)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving products in the price range {MinPrice} to {MaxPrice}.", minPrice, maxPrice);
                throw new RepositoryException("Error occurred while retrieving products by price range.", ex);
            }
        }
    }
}
