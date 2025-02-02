using Microsoft.EntityFrameworkCore;
using WebStoreApp.Data;
using WebStoreApp.Models;
using WebStoreApp.Repositories.Interfaces;
using WebStoreApp.Exceptions; 
using Microsoft.Extensions.Logging;

namespace WebStoreApp.Repositories.Implementations
{
    public class ColorRepository : IColorRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ColorRepository> _logger;

        public ColorRepository(AppDbContext context, ILogger<ColorRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task AddColor(string ColorName)
        {
            try
            {
                Color requestBody = new Color { Name = ColorName };
                _context.Colors.Add(requestBody);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error occurred while adding a color: {ColorName}", ColorName);
                throw new RepositoryException("Error occurred while adding a color.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while adding a color: {ColorName}", ColorName);
                throw new RepositoryException("An unexpected error occurred while adding a color.", ex);
            }
        }

        public async Task DeleteColor(int ColorId)
        {
            try
            {
                var color = await _context.Colors.FindAsync(ColorId);
                if (color == null)
                {
                    _logger.LogWarning("Color with ID {ColorId} not found.", ColorId);
                    throw new RepositoryException($"Color with ID {ColorId} not found.");
                }

                _context.Colors.Remove(color);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error occurred while deleting a color with ID: {ColorId}", ColorId);
                throw new RepositoryException("Error occurred while deleting a color.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while deleting a color with ID: {ColorId}", ColorId);
                throw new RepositoryException("An unexpected error occurred while deleting a color.", ex);
            }
        }

        public async Task<IEnumerable<Color>> GetAllColors()
        {
            try
            {
                return await _context.Colors.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all colors.");
                throw new RepositoryException("Error occurred while retrieving all colors.", ex);
            }
        }

        public async Task<Color> GetColorById(int ColorId)
        {
            try
            {
                var color = await _context.Colors.FindAsync(ColorId);
                if (color == null)
                {
                    _logger.LogWarning("Color with ID {ColorId} not found.", ColorId);
                    throw new RepositoryException($"Color with ID {ColorId} not found.");
                }

                return color;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving the color by ID: {ColorId}", ColorId);
                throw new RepositoryException("Error occurred while retrieving the color by ID.", ex);
            }
        }

        public async Task<Color> GetColorByName(string Name)
        {
            try
            {
                var color = await _context.Colors.FirstOrDefaultAsync(c => c.Name == Name);
                if (color == null)
                {
                    _logger.LogWarning("Color with name '{Name}' not found.", Name);
                    throw new RepositoryException($"Color with name '{Name}' not found.");
                }

                return color;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving the color by name: {Name}", Name);
                throw new RepositoryException("Error occurred while retrieving the color by name.", ex);
            }
        }

        public async Task UpdateColor(Color color)
        {
            try
            {
                _context.Entry(color).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency conflict occurred while updating the color with ID: {ColorId}", color.Id);
                throw new RepositoryException("The color may have been modified by another process.", ex);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error occurred while updating the color with ID: {ColorId}", color.Id);
                throw new RepositoryException("Error occurred while updating the color.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while updating the color with ID: {ColorId}", color.Id);
                throw new RepositoryException("An unexpected error occurred while updating the color.", ex);
            }
        }
    }
}
