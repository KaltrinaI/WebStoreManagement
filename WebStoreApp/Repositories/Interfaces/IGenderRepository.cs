using WebStoreApp.Models;

namespace WebStoreApp.Repositories.Interfaces
{
    public interface IGenderRepository
    {
        Task<Gender> GetGenderById(int GenderId);
        Task<Gender> GetGenderByName(string Name);
        Task<IEnumerable<Gender>> GetAllGenders();
        Task AddGender(string GenderName);
        Task UpdateGender(Gender Gender);
        Task DeleteGender(int GenderId);
    }
}
