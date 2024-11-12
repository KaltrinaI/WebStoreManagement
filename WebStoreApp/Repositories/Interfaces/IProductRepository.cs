using WebStoreApp.Models;

namespace WebStoreApp.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<Product> GetProductById(int Id); 
        Task<IEnumerable<Product>> GetAllProducts();
        Task<IEnumerable<Product>> GetOutOfStockProducts();
        Task<IEnumerable<Product>> GetProductsWithDiscount();
        Task<IEnumerable<Product>> GetProductsByColor(string Color);
        Task<IEnumerable<Product>> GetProductsByBrand(string Brand);
        Task<IEnumerable<Product>> GetProductsByCategory(string Category);
        Task<IEnumerable<Product>> GetProductsByGender(string Gender);
        Task<IEnumerable<Product>> GetProductsBySize(string Size);
        Task AddProduct(Product Product);
        Task UpdateProduct(int ProductId, Product Product);
        Task DeleteProduct(int ProductId);

    }
}
