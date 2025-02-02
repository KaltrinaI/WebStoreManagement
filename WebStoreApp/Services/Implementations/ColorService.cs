using WebStoreApp.Models;
using WebStoreApp.Repositories.Interfaces;
using WebStoreApp.Services.Interfaces;
using WebStoreApp.Exceptions;
using Microsoft.Extensions.Logging;

namespace WebStoreApp.Services.Implementations
{
    public class ColorService : IColorService
    {
        private readonly IColorRepository _repository;
        private readonly ILogger<ColorService> _logger;

        public ColorService(IColorRepository repository, ILogger<ColorService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task AddColor(string colorName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(colorName))
                {
                    _logger.LogWarning("Color name is invalid or empty.");
                    throw new ServiceException("Color name cannot be empty.");
                }

                await _repository.AddColor(colorName);
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, "Error occurred while adding a color: {ColorName}", colorName);
                throw new ServiceException("An error occurred while adding the color.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while adding a color: {ColorName}", colorName);
                throw new ServiceException("An unexpected error occurred while adding the color.", ex);
            }
        }

        public async Task DeleteColor(int colorId)
        {
            try
            {
                if (colorId <= 0)
                {
                    _logger.LogWarning("Invalid color ID: {ColorId}", colorId);
                    throw new ServiceException("Invalid color ID.");
                }

                await _repository.DeleteColor(colorId);
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, "Error occurred while deleting color with ID: {ColorId}", colorId);
                throw new ServiceException("An error occurred while deleting the color.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while deleting color with ID: {ColorId}", colorId);
                throw new ServiceException("An unexpected error occurred while deleting the color.", ex);
            }
        }

        public async Task<IEnumerable<string>> GetAllColors()
        {
            try
            {
                var colors = await _repository.GetAllColors();
                return colors.Select(color => color.Name).ToList();
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all colors.");
                throw new ServiceException("An error occurred while retrieving all colors.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while retrieving all colors.");
                throw new ServiceException("An unexpected error occurred while retrieving all colors.", ex);
            }
        }

        public async Task<string> GetColorById(int colorId)
        {
            try
            {
                if (colorId <= 0)
                {
                    _logger.LogWarning("Invalid color ID: {ColorId}", colorId);
                    throw new ServiceException("Invalid color ID.");
                }

                var color = await _repository.GetColorById(colorId);
                if (color == null || string.IsNullOrEmpty(color.Name))
                {
                    _logger.LogWarning("Color with ID {ColorId} not found.", colorId);
                    throw new ServiceException($"Color with ID {colorId} not found.");
                }

                return color.Name;
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving color by ID: {ColorId}", colorId);
                throw new ServiceException("An error occurred while retrieving the color by ID.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while retrieving color by ID: {ColorId}", colorId);
                throw new ServiceException("An unexpected error occurred while retrieving the color by ID.", ex);
            }
        }

        public async Task<string> GetColorByName(string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    _logger.LogWarning("Color name is invalid or empty.");
                    throw new ServiceException("Color name cannot be empty.");
                }

                var color = await _repository.GetColorByName(name);
                if (color == null || string.IsNullOrEmpty(color.Name))
                {
                    _logger.LogWarning("Color with name '{ColorName}' not found.", name);
                    throw new ServiceException($"Color with name '{name}' not found.");
                }

                return color.Name;
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving color by name: {ColorName}", name);
                throw new ServiceException("An error occurred while retrieving the color by name.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while retrieving color by name: {ColorName}", name);
                throw new ServiceException("An unexpected error occurred while retrieving the color by name.", ex);
            }
        }

        public async Task UpdateColor(int colorId, string colorName)
        {
            try
            {
                if (colorId <= 0)
                {
                    _logger.LogWarning("Invalid color ID: {ColorId}", colorId);
                    throw new ServiceException("Invalid color ID.");
                }

                if (string.IsNullOrWhiteSpace(colorName))
                {
                    _logger.LogWarning("Color name is invalid or empty.");
                    throw new ServiceException("Color name cannot be empty.");
                }

                var color = await _repository.GetColorById(colorId);
                if (color == null || string.IsNullOrEmpty(color.Name))
                {
                    _logger.LogWarning("Color with ID {ColorId} not found.", colorId);
                    throw new ServiceException($"Color with ID {colorId} not found.");
                }

                color.Name = colorName;
                await _repository.UpdateColor(color);
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, "Error occurred while updating color with ID {ColorId}.", colorId);
                throw new ServiceException("An error occurred while updating the color.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while updating color with ID {ColorId}.", colorId);
                throw new ServiceException("An unexpected error occurred while updating the color.", ex);
            }
        }
    }
}
