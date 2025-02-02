using Microsoft.EntityFrameworkCore;
using WebStoreApp.Data;
using WebStoreApp.Models;
using WebStoreApp.Repositories.Interfaces;
using WebStoreApp.Exceptions;

namespace WebStoreApp.Repositories.Implementations
{
    public class BrandRepository : IBrandRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<BrandRepository> _logger;

        public BrandRepository(AppDbContext context, ILogger<BrandRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task AddBrand(string BrandName)
        {
            try
            {
                Brand requestBody = new Brand { Name = BrandName };
                _context.Brands.Add(requestBody);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error occurred while adding a brand: {BrandName}", BrandName);
                throw new RepositoryException("Error occurred while adding a brand.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while adding a brand: {BrandName}", BrandName);
                throw new RepositoryException("An unexpected error occurred while adding a brand.", ex);
            }
        }

        public async Task DeleteBrand(int BrandId)
        {
            try
            {
                var brand = await _context.Brands.FindAsync(BrandId);
                if (brand == null)
                {
                    _logger.LogWarning("Brand with ID {BrandId} not found.", BrandId);
                    throw new RepositoryException($"Brand with ID {BrandId} not found.");
                }

                _context.Brands.Remove(brand);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error occurred while deleting a brand with ID: {BrandId}", BrandId);
                throw new RepositoryException("Error occurred while deleting a brand.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while deleting a brand with ID: {BrandId}", BrandId);
                throw new RepositoryException("An unexpected error occurred while deleting a brand.", ex);
            }
        }

        public async Task<IEnumerable<Brand>> GetAllBrands()
        {
            try
            {
                return await _context.Brands.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all brands.");
                throw new RepositoryException("Error occurred while retrieving all brands.", ex);
            }
        }

        public async Task<Brand> GetBrandById(int BrandId)
        {
            try
            {
                var brand = await _context.Brands.FindAsync(BrandId);
                if (brand == null)
                {
                    _logger.LogWarning("Brand with ID {BrandId} not found.", BrandId);
                    throw new RepositoryException($"Brand with ID {BrandId} not found.");
                }

                return brand;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving the brand by ID: {BrandId}", BrandId);
                throw new RepositoryException("Error occurred while retrieving the brand by ID.", ex);
            }
        }

        public async Task<Brand> GetBrandByName(string Name)
        {
            try
            {
                var brand = await _context.Brands.FirstOrDefaultAsync(b => b.Name == Name);
                if (brand == null)
                {
                    _logger.LogWarning("Brand with name '{Name}' not found.", Name);
                    throw new RepositoryException($"Brand with name '{Name}' not found.");
                }

                return brand;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving the brand by name: {Name}", Name);
                throw new RepositoryException("Error occurred while retrieving the brand by name.", ex);
            }
        }

        public async Task UpdateBrand(Brand brand)
        {
            try
            {
                _context.Entry(brand).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency conflict occurred while updating the brand with ID: {BrandId}", brand.Id);
                throw new RepositoryException("The brand may have been modified by another process.", ex);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error occurred while updating the brand with ID: {BrandId}", brand.Id);
                throw new RepositoryException("Error occurred while updating the brand.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while updating the brand with ID: {BrandId}", brand.Id);
                throw new RepositoryException("An unexpected error occurred while updating the brand.", ex);
            }
        }
    }
}
