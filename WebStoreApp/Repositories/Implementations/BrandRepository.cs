using Microsoft.EntityFrameworkCore;
using WebStoreApp.Data;
using WebStoreApp.Models;
using WebStoreApp.Repositories.Interfaces;

namespace WebStoreApp.Repositories.Implementations
{
    public class BrandRepository : IBrandRepository
    {
        private readonly AppDbContext _context;
        public BrandRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddBrand(string BrandName)
        {
            Brand requestBody = new Brand();
            requestBody.Name = BrandName;
            _context.Brands.Add(requestBody);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteBrand(int BrandId)
        {
            var brand = await _context.Brands.FindAsync(BrandId);
            if (brand != null) {
                _context.Brands.Remove(brand);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Brand>> GetAllBrands()
        {
            return await _context.Brands.ToListAsync();
        }

        public async Task<Brand> GetBrandById(int BrandId)
        {
            var brand = await _context.Brands.FindAsync(BrandId);
            return brand ?? new Brand();
        }

        public async Task<Brand> GetBrandByName(string Name)
        {
            var brand = await _context.Brands.FirstOrDefaultAsync(b=> b.Name==Name);
            return brand ?? new Brand();
        }

        public async Task UpdateBrand(Brand brand)
        {
            _context.Entry(brand).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

    }
}
