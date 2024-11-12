using WebStoreApp.Models;

namespace WebStoreApp.Repositories.Interfaces
{
    public interface IDiscountRepository
    {
        Task<Discount> GetDiscountById(int DiscountId);
        Task<IEnumerable<Discount>> GetDiscountsByName(string name); 
        Task<IEnumerable<Discount>> GetDiscountsByStartingDate(DateTime startDate);
        Task<IEnumerable<Discount>> GetDiscountsByEndingDate(DateTime endDate);
        Task<IEnumerable<Discount>> GetDiscountsByDateRange(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Discount>> GetAllDiscounts();
        Task AddDiscount(Discount Discount);
        Task UpdateDiscount(int DiscountId, Discount Discount);
        Task DeleteDiscount(int DiscountId);
    }
}
