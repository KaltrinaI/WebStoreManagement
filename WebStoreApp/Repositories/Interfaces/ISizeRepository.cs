using WebStoreApp.Models;

namespace WebStoreApp.Repositories.Interfaces
{
    public interface ISizeRepository
    {
        Task<Size> GetSizeById(int SizeId);
        Task<Size> GetSizeByName(string Name);
        Task<IEnumerable<Size>> GetAllSizes();
        Task <string>AddSize(string SizeName);
        Task <string> UpdateSize(int SizzeId, Size size);
        Task <string> DeleteSize(int SizeId);
    }
}
