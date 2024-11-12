using WebStoreApp.DTOs;

namespace WebStoreApp.Services.Interfaces
{
    public interface IDiscountService
    {
        Task<DiscountDTO> GetDiscountById(int DiscountId);
        Task<IEnumerable<DiscountDTO>> GetDiscountsByName(string name);
        Task<IEnumerable<DiscountDTO>> GetDiscountsByStartingDate(DateTime startDate);
        Task<IEnumerable<DiscountDTO>> GetDiscountsByEndingDate(DateTime endDate);
        Task<IEnumerable<DiscountDTO>> GetDiscountsByDateRange(DateTime startDate, DateTime endDate);
        Task<IEnumerable<DiscountDTO>> GetAllDiscounts();
        Task <string> ApplyDiscountToProduct(int productId, int discountId);
        Task<string> ApplyDiscountToCategory(string categoryName, int discountId);
        Task<string> ApplyDiscountToBrand(string brandName, int discountId);
        Task RemoveExpiredDiscounts();
        Task AddDiscount(DiscountDTO Discount);
        Task UpdateDiscount(int DiscountId, DiscountDTO Discount);
        Task DeleteDiscount(int DiscountId);
    }
}
