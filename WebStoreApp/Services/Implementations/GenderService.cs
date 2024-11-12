using WebStoreApp.Models;
using WebStoreApp.Repositories.Interfaces;
using WebStoreApp.Services.Interfaces;

namespace WebStoreApp.Services.Implementations
{
    public class GenderService : IGenderService
    {
        private readonly IGenderRepository _repository;

        public GenderService (IGenderRepository repository)
        {
            _repository = repository;
        }

        public async Task AddGender(string GenderName)
        {
            await _repository.AddGender(GenderName);
        }

        public async Task DeleteGender(int GenderId)
        {
            await _repository.DeleteGender(GenderId);
        }

        public async Task<IEnumerable<string>> GetAllGenders()
        {
            var genders = await _repository.GetAllGenders();
            return genders.Select(gender => gender.Name);
        }

        public async Task<string> GetGenderById(int GenderId)
        {
            var gender = await _repository.GetGenderById(GenderId); 
            return gender.Name;
        }

        public async Task<string> GetGenderByName(string Name)
        {
            var gender = await _repository.GetGenderByName(Name);
            return gender.Name;
        }

        public async Task UpdateGender(int GenderId, string gender)
        {
            var updateGender = await _repository.GetGenderById(GenderId);
            if (updateGender != null)
            {
                updateGender.Name = gender;
                await _repository.UpdateGender(updateGender);
            }
        }
    }
}
