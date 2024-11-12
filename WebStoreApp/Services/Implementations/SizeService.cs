using System.Reflection;
using WebStoreApp.Models;
using WebStoreApp.Repositories.Interfaces;
using WebStoreApp.Services.Interfaces;

namespace WebStoreApp.Services.Implementations
{
    public class SizeService : ISizeService
    {
        private readonly ISizeRepository _repository;

        public SizeService(ISizeRepository repository)
        {
            _repository = repository;
        }

        public async Task<string> AddSize(string SizeName)
        {
           return await _repository.AddSize(SizeName);
        }

        public async Task<string> DeleteSize(int SizeId)
        {
            var sizeDelete = _repository.GetSizeById(SizeId);
            return await _repository.DeleteSize(SizeId);
        }

        public async Task<IEnumerable<string>> GetAllSizes()
        {
            var sizes = await _repository.GetAllSizes();
            return sizes.Select(size => size.Name).ToList();
        }

        public async Task<string> GetSizeById(int SizeId)
        {
            var size = await _repository.GetSizeById(SizeId);
            return size.Name;
        }

        public async Task<string> GetSizeByName(string Name)
        {
            var size = await _repository.GetSizeByName(Name);
            return size.Name;
        }

        public async Task<string> UpdateSize(int SizeId, string size)
        {
            var updateSize= await _repository.GetSizeById(SizeId);
            return await _repository.UpdateSize(SizeId,updateSize);
        }
    }
}
