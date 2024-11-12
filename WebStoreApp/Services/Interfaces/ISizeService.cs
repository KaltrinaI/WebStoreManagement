namespace WebStoreApp.Services.Interfaces
{
    public interface ISizeService
    {
        Task<string> GetSizeById(int SizeId);
        Task<string> GetSizeByName(string Name);
        Task<IEnumerable<string>> GetAllSizes();
        Task <string> AddSize(string SizeName);
        Task<string> UpdateSize(int SizeId, string size);
        Task<string> DeleteSize(int SizeId);
    }
}
