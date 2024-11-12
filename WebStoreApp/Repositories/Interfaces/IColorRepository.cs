using WebStoreApp.Models;

namespace WebStoreApp.Repositories.Interfaces
{
    public interface IColorRepository
    {
        Task<Color> GetColorById(int ColorId);
        Task<Color> GetColorByName(string Name);
        Task<IEnumerable<Color>> GetAllColors();
        Task AddColor(string ColorName);
        Task UpdateColor(Color color);
        Task DeleteColor(int ColorId);
    }
}
