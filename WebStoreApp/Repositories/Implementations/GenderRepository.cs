using Microsoft.EntityFrameworkCore;
using WebStoreApp.Data;
using WebStoreApp.Models;
using WebStoreApp.Repositories.Interfaces;

namespace WebStoreApp.Repositories.Implementations
{
    public class GenderRepository : IGenderRepository
    {
        private readonly AppDbContext _context;
        public GenderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddGender(string GenderName)
        {
            Gender requestBody = new Gender();
            requestBody.Name = GenderName;
            _context.Genders.Add(requestBody);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteGender(int GenderId)
        {
            var gender = await _context.Genders.FindAsync(GenderId);
            if (gender != null)
            {
                _context.Genders.Remove(gender);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Gender>> GetAllGenders()
        {
            return await _context.Genders.ToListAsync();
        }

        public async Task<Gender> GetGenderById(int GenderId)
        {
            var gender = await _context.Genders.FindAsync(GenderId);
            return gender ?? new Gender();
        }

        public async Task<Gender> GetGenderByName(string Name)
        {
            var gender = await _context.Genders.FirstOrDefaultAsync(g => g.Name == Name);
            return gender ?? new Gender();
        }

        public async Task UpdateGender(Gender gender)
        {
            _context.Entry(gender).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
