namespace WebStoreApp.Services.Interfaces
{
    public interface IColorService
    {
        Task<string> GetColorById(int ColorId);
        Task<string> GetColorByName(string Name);
        Task<IEnumerable<string>> GetAllColors();
        Task AddColor(string ColorName);
        Task UpdateColor(int ColorId, string Color);
        Task DeleteColor(int ColorId);
    }
}
