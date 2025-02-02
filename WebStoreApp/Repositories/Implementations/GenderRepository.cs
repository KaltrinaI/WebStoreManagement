using Microsoft.EntityFrameworkCore;
using WebStoreApp.Data;
using WebStoreApp.Models;
using WebStoreApp.Repositories.Interfaces;
using WebStoreApp.Exceptions; // Ensure this namespace is used for RepositoryException
using Microsoft.Extensions.Logging;

namespace WebStoreApp.Repositories.Implementations
{
    public class GenderRepository : IGenderRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<GenderRepository> _logger;

        public GenderRepository(AppDbContext context, ILogger<GenderRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task AddGender(string GenderName)
        {
            try
            {
                Gender requestBody = new Gender { Name = GenderName };
                _context.Genders.Add(requestBody);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error occurred while adding a gender: {GenderName}", GenderName);
                throw new RepositoryException("Error occurred while adding a gender.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while adding a gender: {GenderName}", GenderName);
                throw new RepositoryException("An unexpected error occurred while adding a gender.", ex);
            }
        }

        public async Task DeleteGender(int GenderId)
        {
            try
            {
                var gender = await _context.Genders.FindAsync(GenderId);
                if (gender == null)
                {
                    _logger.LogWarning("Gender with ID {GenderId} not found.", GenderId);
                    throw new RepositoryException($"Gender with ID {GenderId} not found.");
                }

                _context.Genders.Remove(gender);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error occurred while deleting a gender with ID: {GenderId}", GenderId);
                throw new RepositoryException("Error occurred while deleting a gender.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while deleting a gender with ID: {GenderId}", GenderId);
                throw new RepositoryException("An unexpected error occurred while deleting a gender.", ex);
            }
        }

        public async Task<IEnumerable<Gender>> GetAllGenders()
        {
            try
            {
                return await _context.Genders.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all genders.");
                throw new RepositoryException("Error occurred while retrieving all genders.", ex);
            }
        }

        public async Task<Gender> GetGenderById(int GenderId)
        {
            try
            {
                var gender = await _context.Genders.FindAsync(GenderId);
                if (gender == null)
                {
                    _logger.LogWarning("Gender with ID {GenderId} not found.", GenderId);
                    throw new RepositoryException($"Gender with ID {GenderId} not found.");
                }

                return gender;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving the gender by ID: {GenderId}", GenderId);
                throw new RepositoryException("Error occurred while retrieving the gender by ID.", ex);
            }
        }

        public async Task<Gender> GetGenderByName(string Name)
        {
            try
            {
                var gender = await _context.Genders.FirstOrDefaultAsync(g => g.Name == Name);
                if (gender == null)
                {
                    _logger.LogWarning("Gender with name '{Name}' not found.", Name);
                    throw new RepositoryException($"Gender with name '{Name}' not found.");
                }

                return gender;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving the gender by name: {Name}", Name);
                throw new RepositoryException("Error occurred while retrieving the gender by name.", ex);
            }
        }

        public async Task UpdateGender(Gender gender)
        {
            try
            {
                _context.Entry(gender).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency conflict occurred while updating the gender with ID: {GenderId}", gender.Id);
                throw new RepositoryException("The gender may have been modified by another process.", ex);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error occurred while updating the gender with ID: {GenderId}", gender.Id);
                throw new RepositoryException("Error occurred while updating the gender.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while updating the gender with ID: {GenderId}", gender.Id);
                throw new RepositoryException("An unexpected error occurred while updating the gender.", ex);
            }
        }
    }
}
