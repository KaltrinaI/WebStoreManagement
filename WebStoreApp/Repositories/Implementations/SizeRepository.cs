using Microsoft.EntityFrameworkCore;
using WebStoreApp.Data;
using WebStoreApp.Models;
using WebStoreApp.Repositories.Interfaces;
using WebStoreApp.Exceptions;
using Microsoft.Extensions.Logging;

namespace WebStoreApp.Repositories.Implementations
{
    public class SizeRepository : ISizeRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<SizeRepository> _logger;

        public SizeRepository(AppDbContext context, ILogger<SizeRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<string> AddSize(string sizeName)
        {
            try
            {
                var size = new Size { Name = sizeName };
                await _context.Sizes.AddAsync(size);
                await _context.SaveChangesAsync();
                return "Size created successfully!";
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error occurred while adding a size: {SizeName}", sizeName);
                throw new RepositoryException("Error occurred while adding a size.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while adding a size: {SizeName}", sizeName);
                throw new RepositoryException("An unexpected error occurred while adding a size.", ex);
            }
        }

        public async Task<string> DeleteSize(int sizeId)
        {
            try
            {
                var size = await _context.Sizes.FindAsync(sizeId);
                if (size == null)
                {
                    _logger.LogWarning("Size with ID {SizeId} not found.", sizeId);
                    return "The size does not exist!";
                }

                _context.Sizes.Remove(size);
                await _context.SaveChangesAsync();
                return "Size deleted successfully!";
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error occurred while deleting a size with ID {SizeId}.", sizeId);
                throw new RepositoryException("Error occurred while deleting a size.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while deleting a size with ID {SizeId}.", sizeId);
                throw new RepositoryException("An unexpected error occurred while deleting a size.", ex);
            }
        }

        public async Task<IEnumerable<Size>> GetAllSizes()
        {
            try
            {
                return await _context.Sizes.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all sizes.");
                throw new RepositoryException("Error occurred while retrieving all sizes.", ex);
            }
        }

        public async Task<Size> GetSizeById(int sizeId)
        {
            try
            {
                var size = await _context.Sizes.FindAsync(sizeId);
                if (size == null)
                {
                    _logger.LogWarning("Size with ID {SizeId} not found.", sizeId);
                    throw new RepositoryException($"Size with ID {sizeId} not found.");
                }

                return size;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving size by ID {SizeId}.", sizeId);
                throw new RepositoryException("Error occurred while retrieving size by ID.", ex);
            }
        }

        public async Task<Size> GetSizeByName(string name)
        {
            try
            {
                var size = await _context.Sizes.FirstOrDefaultAsync(s => s.Name == name);
                if (size == null)
                {
                    _logger.LogWarning("Size with name '{SizeName}' not found.", name);
                    throw new RepositoryException($"Size with name '{name}' not found.");
                }

                return size;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving size by name: {SizeName}.", name);
                throw new RepositoryException("Error occurred while retrieving size by name.", ex);
            }
        }

        public async Task<string> UpdateSize(int sizeId, Size size)
        {
            try
            {
                var updateSize = await _context.Sizes.FindAsync(sizeId);
                if (updateSize == null)
                {
                    _logger.LogWarning("Size with ID {SizeId} not found.", sizeId);
                    return "Size does not exist!";
                }

                updateSize.Name = size.Name;
                _context.Entry(updateSize).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return "Size updated successfully!";
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error occurred while updating size with ID {SizeId}.", sizeId);
                throw new RepositoryException("Error occurred while updating the size.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while updating size with ID {SizeId}.", sizeId);
                throw new RepositoryException("An unexpected error occurred while updating the size.", ex);
            }
        }
    }
}
