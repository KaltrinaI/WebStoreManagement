using WebStoreApp.Models;

namespace WebStoreApp.Services.Interfaces
{
    public interface IGenderService
    {
        Task<string> GetGenderById(int GenderId);
        Task<string> GetGenderByName(string Name);
        Task<IEnumerable<string >> GetAllGenders();
        Task AddGender(string GenderName);
        Task UpdateGender(int GenderId, string gender);
        Task DeleteGender(int GenderId);
    }
}
