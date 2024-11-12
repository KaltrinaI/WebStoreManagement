using WebStoreApp.Models;

namespace WebStoreApp.Repositories.Interfaces
{
    public interface IBrandRepository
    {
        Task<Brand> GetBrandById(int BrandId);
        Task<Brand> GetBrandByName(string Name);
        Task<IEnumerable<Brand>> GetAllBrands();
        Task AddBrand(string BrandName);
        Task  UpdateBrand(Brand Brand);
        Task DeleteBrand(int BrandId);
    }
}
