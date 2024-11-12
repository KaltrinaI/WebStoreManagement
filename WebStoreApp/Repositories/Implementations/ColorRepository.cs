using Microsoft.EntityFrameworkCore;
using WebStoreApp.Data;
using WebStoreApp.Models;
using WebStoreApp.Repositories.Interfaces;

namespace WebStoreApp.Repositories.Implementations
{
    public class ColorRepository : IColorRepository
    {
        private readonly AppDbContext _context;
        public ColorRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddColor(string ColorName)
        {
            Color requestBody = new Color();
            requestBody.Name = ColorName;
            _context.Colors.Add(requestBody);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteColor(int ColorId)
        {
            var color = await _context.Colors.FindAsync(ColorId);
            if (color != null)
            {
                _context.Colors.Remove(color);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Color>> GetAllColors()
        {
            return await _context.Colors.ToListAsync();
        }

        public async Task<Color> GetColorById(int ColorId)
        {
            var color = await _context.Colors.FindAsync(ColorId);
            return color ?? new Color();
        }

        public async Task<Color> GetColorByName(string Name)
        {
            var color = await _context.Colors.FirstOrDefaultAsync(c => c.Name == Name);
            return color ?? new Color();
        }

        public async Task UpdateColor(Color color)
        {
            _context.Entry(color).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
