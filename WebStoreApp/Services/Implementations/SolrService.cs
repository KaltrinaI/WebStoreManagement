using SolrNet;
using WebStoreApp.SolrIndexModels;
using WebStoreApp.Services.Interfaces;

public class SolrService : ISolrService
{
    private readonly ISolrOperations<ProductIndex> _solr;

    public SolrService(ISolrOperations<ProductIndex> solr)
    {
        _solr = solr;
    }

    public async Task IndexProductAsync(ProductIndex product)
    {
        await _solr.AddAsync(product);
        await _solr.CommitAsync();
    }

    public async Task<List<ProductIndex>> SearchProductsAsync(string query)
    {
        var results = await _solr.QueryAsync(new SolrQuery(query));
        return results.ToList();
    }
}
