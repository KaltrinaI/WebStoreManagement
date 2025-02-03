using Microsoft.EntityFrameworkCore;
using WebStoreApp.Data;
using WebStoreApp.Models;
using WebStoreApp.Repositories.Interfaces;
using WebStoreApp.Exceptions; 
using Microsoft.Extensions.Logging;

namespace WebStoreApp.Repositories.Implementations
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DiscountRepository> _logger;

        public DiscountRepository(AppDbContext context, ILogger<DiscountRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task AddDiscount(Discount Discount)
        {
            try
            {
                Discount requestBody = new Discount
                {
                    Name = Discount.Name,
                    StartDate = Discount.StartDate,
                    EndDate = Discount.EndDate,
                    DiscountPercentage = Discount.DiscountPercentage
                };

                _context.Discounts.Add(requestBody);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error occurred while adding a discount: {DiscountName}", Discount.Name);
                throw new RepositoryException("Error occurred while adding a discount.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while adding a discount: {DiscountName}", Discount.Name);
                throw new RepositoryException("An unexpected error occurred while adding a discount.", ex);
            }
        }

        public async Task DeleteDiscount(int DiscountId)
        {
            try
            {
                var discount = await _context.Discounts.FindAsync(DiscountId);
                if (discount == null)
                {
                    _logger.LogWarning("Discount with ID {DiscountId} not found.", DiscountId);
                    throw new RepositoryException($"Discount with ID {DiscountId} not found.");
                }

                _context.Discounts.Remove(discount);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error occurred while deleting a discount with ID: {DiscountId}", DiscountId);
                throw new RepositoryException("Error occurred while deleting a discount.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while deleting a discount with ID: {DiscountId}", DiscountId);
                throw new RepositoryException("An unexpected error occurred while deleting a discount.", ex);
            }
        }

        public async Task<IEnumerable<Discount>> GetAllDiscounts()
        {
            try
            {
                return await _context.Discounts.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all discounts.");
                throw new RepositoryException("Error occurred while retrieving all discounts.", ex);
            }
        }

        public async Task<Discount> GetDiscountById(int DiscountId)
        {
            try
            {
                var discount = await _context.Discounts.FindAsync(DiscountId);
                if (discount == null)
                {
                    _logger.LogWarning("Discount with ID {DiscountId} not found.", DiscountId);
                    throw new RepositoryException($"Discount with ID {DiscountId} not found.");
                }

                return discount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving the discount by ID: {DiscountId}", DiscountId);
                throw new RepositoryException("Error occurred while retrieving the discount by ID.", ex);
            }
        }

        public async Task<IEnumerable<Discount>> GetDiscountsByDateRange(DateTime startDate, DateTime endDate)
        {
            try
            {
                return await _context.Discounts
                                     .Where(d => d.StartDate >= startDate && d.EndDate <= endDate)
                                     .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving discounts by date range: {StartDate} to {EndDate}", startDate, endDate);
                throw new RepositoryException("Error occurred while retrieving discounts by date range.", ex);
            }
        }

        public async Task<IEnumerable<Discount>> GetDiscountsByEndingDate(DateTime endDate)
        {
            try
            {
                return await _context.Discounts
                                     .Where(d => d.EndDate.Date == endDate.Date)
                                     .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving discounts ending on: {EndDate}", endDate);
                throw new RepositoryException("Error occurred while retrieving discounts by ending date.", ex);
            }
        }

        public async Task<IEnumerable<Discount>> GetDiscountsByName(string name)
        {
            try
            {
                return await _context.Discounts
                                     .Where(d => d.Name.Contains(name))
                                     .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving discounts by name: {Name}", name);
                throw new RepositoryException("Error occurred while retrieving discounts by name.", ex);
            }
        }

        public async Task<IEnumerable<Discount>> GetDiscountsByStartingDate(DateTime startDate)
        {
            try
            {
                return await _context.Discounts
                                     .Where(d => d.StartDate.Date == startDate.Date)
                                     .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving discounts starting on: {StartDate}", startDate);
                throw new RepositoryException("Error occurred while retrieving discounts by starting date.", ex);
            }
        }

        public async Task UpdateDiscount(int DiscountId, Discount Discount)
        {
            try
            {
                var discount = await _context.Discounts.FindAsync(DiscountId);
                if (discount == null)
                {
                    _logger.LogWarning("Discount with ID {DiscountId} not found.", DiscountId);
                    throw new RepositoryException($"Discount with ID {DiscountId} not found.");
                }

                discount.Name = Discount.Name;
                discount.StartDate = Discount.StartDate;
                discount.EndDate = Discount.EndDate;
                discount.DiscountPercentage = Discount.DiscountPercentage;

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency conflict occurred while updating the discount with ID: {DiscountId}", DiscountId);
                throw new RepositoryException("The discount may have been modified by another process.", ex);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error occurred while updating the discount with ID: {DiscountId}", DiscountId);
                throw new RepositoryException("Error occurred while updating the discount.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while updating the discount with ID: {DiscountId}", DiscountId);
                throw new RepositoryException("An unexpected error occurred while updating the discount.", ex);
            }
        }
    }
}
