using WebStoreApp.Models;
using WebStoreApp.Repositories.Interfaces;
using WebStoreApp.Services.Interfaces;
using WebStoreApp.Exceptions; // For ServiceException
using Microsoft.Extensions.Logging;

namespace WebStoreApp.Services.Implementations
{
    public class BrandService : IBrandService
    {
        private readonly IBrandRepository _repository;
        private readonly ILogger<BrandService> _logger;

        public BrandService(IBrandRepository repository, ILogger<BrandService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task AddBrand(string brandName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(brandName))
                {
                    _logger.LogWarning("Brand name is invalid or empty.");
                    throw new ServiceException("Brand name cannot be empty.");
                }

                await _repository.AddBrand(brandName);
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, "Error occurred while adding a brand: {BrandName}", brandName);
                throw new ServiceException("An error occurred while adding the brand.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while adding a brand: {BrandName}", brandName);
                throw new ServiceException("An unexpected error occurred while adding the brand.", ex);
            }
        }

        public async Task DeleteBrand(int brandId)
        {
            try
            {
                if (brandId <= 0)
                {
                    _logger.LogWarning("Invalid brand ID: {BrandId}", brandId);
                    throw new ServiceException("Invalid brand ID.");
                }

                await _repository.DeleteBrand(brandId);
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, "Error occurred while deleting brand with ID: {BrandId}", brandId);
                throw new ServiceException("An error occurred while deleting the brand.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while deleting brand with ID: {BrandId}", brandId);
                throw new ServiceException("An unexpected error occurred while deleting the brand.", ex);
            }
        }

        public async Task<IEnumerable<string>> GetAllBrands()
        {
            try
            {
                var brands = await _repository.GetAllBrands();
                return brands.Select(brand => brand.Name).ToList();
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all brands.");
                throw new ServiceException("An error occurred while retrieving all brands.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while retrieving all brands.");
                throw new ServiceException("An unexpected error occurred while retrieving all brands.", ex);
            }
        }

        public async Task<string> GetBrandById(int brandId)
        {
            try
            {
                if (brandId <= 0)
                {
                    _logger.LogWarning("Invalid brand ID: {BrandId}", brandId);
                    throw new ServiceException("Invalid brand ID.");
                }

                var brand = await _repository.GetBrandById(brandId);
                if (brand == null || string.IsNullOrEmpty(brand.Name))
                {
                    _logger.LogWarning("Brand with ID {BrandId} not found.", brandId);
                    throw new ServiceException($"Brand with ID {brandId} not found.");
                }

                return brand.Name;
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving brand by ID: {BrandId}", brandId);
                throw new ServiceException("An error occurred while retrieving the brand by ID.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while retrieving brand by ID: {BrandId}", brandId);
                throw new ServiceException("An unexpected error occurred while retrieving the brand by ID.", ex);
            }
        }

        public async Task<string> GetBrandByName(string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    _logger.LogWarning("Brand name is invalid or empty.");
                    throw new ServiceException("Brand name cannot be empty.");
                }

                var brand = await _repository.GetBrandByName(name);
                if (brand == null || string.IsNullOrEmpty(brand.Name))
                {
                    _logger.LogWarning("Brand with name '{BrandName}' not found.", name);
                    throw new ServiceException($"Brand with name '{name}' not found.");
                }

                return brand.Name;
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving brand by name: {BrandName}", name);
                throw new ServiceException("An error occurred while retrieving the brand by name.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while retrieving brand by name: {BrandName}", name);
                throw new ServiceException("An unexpected error occurred while retrieving the brand by name.", ex);
            }
        }

        public async Task UpdateBrand(int brandId, string brandName)
        {
            try
            {
                if (brandId <= 0)
                {
                    _logger.LogWarning("Invalid brand ID: {BrandId}", brandId);
                    throw new ServiceException("Invalid brand ID.");
                }

                if (string.IsNullOrWhiteSpace(brandName))
                {
                    _logger.LogWarning("Brand name is invalid or empty.");
                    throw new ServiceException("Brand name cannot be empty.");
                }

                var brand = await _repository.GetBrandById(brandId);
                if (brand == null || string.IsNullOrEmpty(brand.Name))
                {
                    _logger.LogWarning("Brand with ID {BrandId} not found.", brandId);
                    throw new ServiceException($"Brand with ID {brandId} not found.");
                }

                brand.Name = brandName;
                await _repository.UpdateBrand(brand);
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, "Error occurred while updating brand with ID {BrandId}.", brandId);
                throw new ServiceException("An error occurred while updating the brand.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while updating brand with ID {BrandId}.", brandId);
                throw new ServiceException("An unexpected error occurred while updating the brand.", ex);
            }
        }
    }
}
