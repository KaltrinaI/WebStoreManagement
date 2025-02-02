using WebStoreApp.Models;
using WebStoreApp.Repositories.Interfaces;
using WebStoreApp.Services.Interfaces;
using WebStoreApp.Exceptions;
using Microsoft.Extensions.Logging;

namespace WebStoreApp.Services.Implementations
{
    public class GenderService : IGenderService
    {
        private readonly IGenderRepository _repository;
        private readonly ILogger<GenderService> _logger;

        public GenderService(IGenderRepository repository, ILogger<GenderService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task AddGender(string genderName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(genderName))
                {
                    _logger.LogWarning("Gender name is invalid or empty.");
                    throw new ServiceException("Gender name cannot be empty.");
                }

                await _repository.AddGender(genderName);
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, "Error occurred while adding a gender: {GenderName}", genderName);
                throw new ServiceException("An error occurred while adding the gender.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while adding a gender: {GenderName}", genderName);
                throw new ServiceException("An unexpected error occurred while adding the gender.", ex);
            }
        }

        public async Task DeleteGender(int genderId)
        {
            try
            {
                if (genderId <= 0)
                {
                    _logger.LogWarning("Invalid gender ID: {GenderId}", genderId);
                    throw new ServiceException("Invalid gender ID.");
                }

                await _repository.DeleteGender(genderId);
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, "Error occurred while deleting gender with ID: {GenderId}", genderId);
                throw new ServiceException("An error occurred while deleting the gender.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while deleting gender with ID: {GenderId}", genderId);
                throw new ServiceException("An unexpected error occurred while deleting the gender.", ex);
            }
        }

        public async Task<IEnumerable<string>> GetAllGenders()
        {
            try
            {
                var genders = await _repository.GetAllGenders();
                return genders.Select(gender => gender.Name);
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all genders.");
                throw new ServiceException("An error occurred while retrieving all genders.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while retrieving all genders.");
                throw new ServiceException("An unexpected error occurred while retrieving all genders.", ex);
            }
        }

        public async Task<string> GetGenderById(int genderId)
        {
            try
            {
                if (genderId <= 0)
                {
                    _logger.LogWarning("Invalid gender ID: {GenderId}", genderId);
                    throw new ServiceException("Invalid gender ID.");
                }

                var gender = await _repository.GetGenderById(genderId);
                if (gender == null || string.IsNullOrEmpty(gender.Name))
                {
                    _logger.LogWarning("Gender with ID {GenderId} not found.", genderId);
                    throw new ServiceException($"Gender with ID {genderId} not found.");
                }

                return gender.Name;
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving gender by ID: {GenderId}", genderId);
                throw new ServiceException("An error occurred while retrieving the gender by ID.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while retrieving gender by ID: {GenderId}", genderId);
                throw new ServiceException("An unexpected error occurred while retrieving the gender by ID.", ex);
            }
        }

        public async Task<string> GetGenderByName(string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    _logger.LogWarning("Gender name is invalid or empty.");
                    throw new ServiceException("Gender name cannot be empty.");
                }

                var gender = await _repository.GetGenderByName(name);
                if (gender == null || string.IsNullOrEmpty(gender.Name))
                {
                    _logger.LogWarning("Gender with name '{GenderName}' not found.", name);
                    throw new ServiceException($"Gender with name '{name}' not found.");
                }

                return gender.Name;
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving gender by name: {GenderName}", name);
                throw new ServiceException("An error occurred while retrieving the gender by name.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while retrieving gender by name: {GenderName}", name);
                throw new ServiceException("An unexpected error occurred while retrieving the gender by name.", ex);
            }
        }

        public async Task UpdateGender(int genderId, string genderName)
        {
            try
            {
                if (genderId <= 0)
                {
                    _logger.LogWarning("Invalid gender ID: {GenderId}", genderId);
                    throw new ServiceException("Invalid gender ID.");
                }

                if (string.IsNullOrWhiteSpace(genderName))
                {
                    _logger.LogWarning("Gender name is invalid or empty.");
                    throw new ServiceException("Gender name cannot be empty.");
                }

                var gender = await _repository.GetGenderById(genderId);
                if (gender == null || string.IsNullOrEmpty(gender.Name))
                {
                    _logger.LogWarning("Gender with ID {GenderId} not found.", genderId);
                    throw new ServiceException($"Gender with ID {genderId} not found.");
                }

                gender.Name = genderName;
                await _repository.UpdateGender(gender);
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, "Error occurred while updating gender with ID {GenderId}.", genderId);
                throw new ServiceException("An error occurred while updating the gender.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while updating gender with ID {GenderId}.", genderId);
                throw new ServiceException("An unexpected error occurred while updating the gender.", ex);
            }
        }
    }
}
