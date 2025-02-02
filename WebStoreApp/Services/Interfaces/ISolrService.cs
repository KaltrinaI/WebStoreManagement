using WebStoreApp.SolrIndexModels;

namespace WebStoreApp.Services.Interfaces
{
    public interface ISolrService
    {
        Task IndexProductAsync(ProductIndex product);
        Task<List<ProductIndex>> SearchProductsAsync(string query);

    }
}
