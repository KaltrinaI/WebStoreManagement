using WebStoreApp.Models;
using WebStoreApp.Repositories.Interfaces;
using WebStoreApp.Services.Interfaces;
using WebStoreApp.Exceptions;
using Microsoft.Extensions.Logging;

namespace WebStoreApp.Services.Implementations
{
    public class SizeService : ISizeService
    {
        private readonly ISizeRepository _repository;
        private readonly ILogger<SizeService> _logger;

        public SizeService(ISizeRepository repository, ILogger<SizeService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<string> AddSize(string sizeName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(sizeName))
                {
                    _logger.LogWarning("Size name is invalid or empty.");
                    throw new ServiceException("Size name cannot be empty.");
                }

                return await _repository.AddSize(sizeName);
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, "Error occurred while adding a size: {SizeName}", sizeName);
                throw new ServiceException("An error occurred while adding the size.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while adding a size: {SizeName}", sizeName);
                throw new ServiceException("An unexpected error occurred while adding the size.", ex);
            }
        }

        public async Task<string> DeleteSize(int sizeId)
        {
            try
            {
                if (sizeId <= 0)
                {
                    _logger.LogWarning("Invalid size ID: {SizeId}", sizeId);
                    throw new ServiceException("Invalid size ID.");
                }

                var size = await _repository.GetSizeById(sizeId);
                if (size == null || string.IsNullOrEmpty(size.Name))
                {
                    _logger.LogWarning("Size with ID {SizeId} not found.", sizeId);
                    throw new ServiceException($"Size with ID {sizeId} not found.");
                }

                return await _repository.DeleteSize(sizeId);
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, "Error occurred while deleting size with ID: {SizeId}", sizeId);
                throw new ServiceException("An error occurred while deleting the size.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while deleting size with ID: {SizeId}", sizeId);
                throw new ServiceException("An unexpected error occurred while deleting the size.", ex);
            }
        }

        public async Task<IEnumerable<string>> GetAllSizes()
        {
            try
            {
                var sizes = await _repository.GetAllSizes();
                return sizes.Select(size => size.Name).ToList();
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all sizes.");
                throw new ServiceException("An error occurred while retrieving all sizes.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while retrieving all sizes.");
                throw new ServiceException("An unexpected error occurred while retrieving all sizes.", ex);
            }
        }

        public async Task<string> GetSizeById(int sizeId)
        {
            try
            {
                if (sizeId <= 0)
                {
                    _logger.LogWarning("Invalid size ID: {SizeId}", sizeId);
                    throw new ServiceException("Invalid size ID.");
                }

                var size = await _repository.GetSizeById(sizeId);
                if (size == null || string.IsNullOrEmpty(size.Name))
                {
                    _logger.LogWarning("Size with ID {SizeId} not found.", sizeId);
                    throw new ServiceException($"Size with ID {sizeId} not found.");
                }

                return size.Name;
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving size by ID: {SizeId}", sizeId);
                throw new ServiceException("An error occurred while retrieving the size by ID.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while retrieving size by ID: {SizeId}", sizeId);
                throw new ServiceException("An unexpected error occurred while retrieving the size by ID.", ex);
            }
        }

        public async Task<string> GetSizeByName(string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    _logger.LogWarning("Size name is invalid or empty.");
                    throw new ServiceException("Size name cannot be empty.");
                }

                var size = await _repository.GetSizeByName(name);
                if (size == null || string.IsNullOrEmpty(size.Name))
                {
                    _logger.LogWarning("Size with name '{SizeName}' not found.", name);
                    throw new ServiceException($"Size with name '{name}' not found.");
                }

                return size.Name;
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving size by name: {SizeName}", name);
                throw new ServiceException("An error occurred while retrieving the size by name.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while retrieving size by name: {SizeName}", name);
                throw new ServiceException("An unexpected error occurred while retrieving the size by name.", ex);
            }
        }

        public async Task<string> UpdateSize(int sizeId, string sizeName)
        {
            try
            {
                if (sizeId <= 0)
                {
                    _logger.LogWarning("Invalid size ID: {SizeId}", sizeId);
                    throw new ServiceException("Invalid size ID.");
                }

                if (string.IsNullOrWhiteSpace(sizeName))
                {
                    _logger.LogWarning("Size name is invalid or empty.");
                    throw new ServiceException("Size name cannot be empty.");
                }

                var size = await _repository.GetSizeById(sizeId);
                if (size == null || string.IsNullOrEmpty(size.Name))
                {
                    _logger.LogWarning("Size with ID {SizeId} not found.", sizeId);
                    throw new ServiceException($"Size with ID {sizeId} not found.");
                }

                size.Name = sizeName;
                return await _repository.UpdateSize(sizeId, size);
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, "Error occurred while updating size with ID {SizeId}.", sizeId);
                throw new ServiceException("An error occurred while updating the size.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while updating size with ID {SizeId}.", sizeId);
                throw new ServiceException("An unexpected error occurred while updating the size.", ex);
            }
        }
    }
}
