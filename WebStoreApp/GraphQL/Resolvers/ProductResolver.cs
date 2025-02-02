using WebStoreApp.DTOs;
using WebStoreApp.Services.Interfaces;

namespace WebStoreApp.GraphQL.Resolvers
{
    public class ProductResolver
    {
        private readonly IProductService _productService;

        public ProductResolver(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<int> GetStockLevel([Parent] ProductDTO product)
        {
            return product.Quantity * 10;
        }

    }

}
