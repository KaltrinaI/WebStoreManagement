using WebStoreApp.DTOs;

namespace WebStoreApp.Services.Interfaces
{
    public interface IProductService
    {
        Task<ProductDTO> GetProductById(int Id);
        Task<IEnumerable<ProductDTO>> GetAllProducts();
        Task<IEnumerable<ProductDTO>> GetOutOfStockProducts();
        Task<IEnumerable<ProductDTO>> GetProductsWithDiscount();
        Task<IEnumerable<ProductDTO>> GetProductsByColor(string Color);
        Task<IEnumerable<ProductDTO>> GetProductsByBrand(string Brand);
        Task<IEnumerable<ProductDTO>> GetProductsByCategory(string Category);
        Task<IEnumerable<ProductDTO>> GetProductsByGender(string Gender);
        Task<IEnumerable<ProductDTO>> GetProductsBySize(string Size);
        Task<IEnumerable<ProductDTO>> AdvancedProductSearch(string? category, string? gender, string? brand, double? minPrice, double? maxPrice,
            string? size, string? color, bool? inStock);
        Task AddProduct(ProductDTO Product);
        Task UpdateProduct(int ProductId, ProductDTO Product);
        Task DeleteProduct(int ProductId);
        Task<ProductQuantityDTO> GetRealTimeProductQuantity(int productId);
    }
}
