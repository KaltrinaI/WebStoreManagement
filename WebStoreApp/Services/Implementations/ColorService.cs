using System.Reflection;
using WebStoreApp.Models;
using WebStoreApp.Repositories.Interfaces;
using WebStoreApp.Services.Interfaces;

namespace WebStoreApp.Services.Implementations
{
    public class ColorService : IColorService
    {
        private readonly IColorRepository _repository;

        public ColorService(IColorRepository repository)
        {
            _repository = repository;
        }

        public async Task AddColor(string ColorName)
        {
            await _repository.AddColor(ColorName);
        }

        public async Task DeleteColor(int ColorId)
        {
            await _repository.DeleteColor(ColorId);
        }

        public async Task<IEnumerable<string>> GetAllColors()
        {
            var colors = await _repository.GetAllColors();
            return colors.Select(color => color.Name).ToList();

        }

        public async Task<string> GetColorById(int ColorId)
        {
            var color = await _repository.GetColorById(ColorId);
            return color.Name;
        }

        public async Task<string> GetColorByName(string Name)
        {
            var color = await _repository.GetColorByName(Name);
            return color.Name;
        }

        public async Task UpdateColor(int ColorId, string color)
        {
            var updateColor= await _repository.GetColorById(ColorId);
            if (updateColor != null)
            {
                updateColor.Name = color;
                await _repository.UpdateColor(updateColor);
            }
        }
    }
}
