using WebStoreApp.Models;

namespace WebStoreApp.Services.Interfaces
{
    public interface IBrandService
    {
        Task<string> GetBrandById(int BrandId);
        Task<string> GetBrandByName(string Name);
        Task<IEnumerable<string>> GetAllBrands();
        Task AddBrand(string BrandName);
        Task UpdateBrand(int BrandId, string BrandName);
        Task DeleteBrand(int BrandId);
    }
}
