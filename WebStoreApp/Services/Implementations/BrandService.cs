using WebStoreApp.Data;
using WebStoreApp.Models;
using WebStoreApp.Repositories.Interfaces;
using WebStoreApp.Services.Interfaces;

namespace WebStoreApp.Services.Implementations
{
    public class BrandService: IBrandService
    {
        private readonly IBrandRepository _repository;

        public BrandService(IBrandRepository repository)
        {
            _repository = repository;
        }


        public async Task AddBrand(string BrandName)
        {
            await _repository.AddBrand(BrandName);
        }

        public async Task DeleteBrand(int BrandId)
        {
            await _repository.DeleteBrand(BrandId);
        }

        public async Task<IEnumerable<string>> GetAllBrands()
        {
            var brands = await _repository.GetAllBrands();
            return brands.Select(brand => brand.Name).ToList();
        }

        public async Task<string> GetBrandById(int BrandId)
        {
            var brand = await _repository.GetBrandById(BrandId);
            return brand.Name;
        }

        public async Task<string> GetBrandByName(string Name)
        {
            var brand = await _repository.GetBrandByName(Name);
            return brand.Name;
        }

        public async Task UpdateBrand(int BrandId, string BrandName)
        {
            var brand = await _repository.GetBrandById(BrandId);
            if (brand != null)
            {
                brand.Name = BrandName;
                await _repository.UpdateBrand(brand);
            }
        }
    }
}
