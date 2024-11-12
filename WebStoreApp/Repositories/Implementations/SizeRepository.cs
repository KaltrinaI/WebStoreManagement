using Microsoft.EntityFrameworkCore;
using WebStoreApp.Data;
using WebStoreApp.Models;
using WebStoreApp.Repositories.Interfaces;

namespace WebStoreApp.Repositories.Implementations
{
    public class SizeRepository : ISizeRepository
    {
        private readonly AppDbContext _context;
        public SizeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<string> AddSize(string SizeName)
        {
            Size requestBody = new Size();
            requestBody.Name = SizeName;
            _context.Sizes.Add(requestBody);
            await _context.SaveChangesAsync();
            return "Size created Successfully!";
        }

        public async Task<string> DeleteSize(int SizeId)
        {
            var size = await _context.Sizes.FindAsync(SizeId);
            if (size == null)
            {
                return "The Size does not exist!";
            }

            _context.Sizes.Remove(size);
            await _context.SaveChangesAsync();
            return "Size Deleted Succesfully!";

        }

        public async Task<IEnumerable<Size>> GetAllSizes()
        {
            return await _context.Sizes.ToListAsync();
        }

        public async Task<Size> GetSizeById(int SizeId)
        {
            var size = await _context.Sizes.FindAsync(SizeId);
            return size ?? new Size();
        }

        public async Task<Size> GetSizeByName(string Name)
        {
            var size = await _context.Sizes.FirstOrDefaultAsync(s => s.Name == Name);
            return size ?? new Size();
        }

        public async Task<string> UpdateSize(int SizeId, Size size)
        {
            var updateSize = await _context.Sizes.FindAsync(SizeId);
            if (updateSize == null)
            {
                return "Size does not exist!";
            }
            updateSize.Name = size.Name;
            _context.Entry(size).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return "Size updated Successfully!";
        }
    }
}
