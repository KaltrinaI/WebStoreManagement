using Microsoft.EntityFrameworkCore;
using WebStoreApp.Data;
using WebStoreApp.Models;
using WebStoreApp.Repositories.Interfaces;

namespace WebStoreApp.Repositories.Implementations
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly AppDbContext _context;
        public DiscountRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddDiscount(Discount Discount)
        {
            Discount requestBody = new Discount();
            requestBody.Name = Discount.Name;
            requestBody.StartDate = Discount.StartDate;
            requestBody.EndDate = Discount.EndDate;
            requestBody.DisountPercentage = Discount.DisountPercentage;
            _context.Discounts.Add(requestBody);
            await _context.SaveChangesAsync();

        }

        public async Task DeleteDiscount(int DiscountId)
        {
            var discount = await _context.Discounts.FindAsync(DiscountId);
            if (discount != null)
            {
                _context.Discounts.Remove(discount);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Discount>> GetAllDiscounts()
        {
            return await _context.Discounts.ToListAsync();
        }

        public async Task<Discount> GetDiscountById(int DiscountId)
        {
            var discount = await _context.Discounts.FindAsync(DiscountId);
            return discount ?? new Discount();
        }

        public async Task<IEnumerable<Discount>> GetDiscountsByDateRange(DateTime startDate, DateTime endDate)
        {
            return await _context.Discounts
                                 .Where(d => d.StartDate >= startDate && d.EndDate <= endDate)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<Discount>> GetDiscountsByEndingDate(DateTime endDate)
        {
            return await _context.Discounts
                  .Where(d => d.EndDate.Date == endDate.Date)
                  .ToListAsync();
        }

        public async Task<IEnumerable<Discount>> GetDiscountsByName(string name)
        {
            return await _context.Discounts
                     .Where(d => d.Name.Contains(name))
                     .ToListAsync();
        }

        public async Task<IEnumerable<Discount>> GetDiscountsByStartingDate(DateTime startDate)
        {
            return await _context.Discounts
                      .Where(d => d.StartDate.Date == startDate.Date)
                      .ToListAsync();
        }

        public async Task UpdateDiscount(int DiscountId, Discount Discount)
        {
            var discount = await _context.Discounts.FindAsync(DiscountId);
            if (discount != null)
            {
                discount.Name = Discount.Name;
                discount.StartDate = Discount.StartDate;
                discount.EndDate = Discount.EndDate;
                discount.DisountPercentage = Discount.DisountPercentage;
                await _context.SaveChangesAsync();
            }
        }
    }
}
