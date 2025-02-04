using SolrNet;
using SolrNet.Commands.Parameters;
using WebStoreApp.Models;

public class SolrService
{
    private readonly ISolrOperations<Product> _solr;

    public SolrService(ISolrOperations<Product> solr)
    {
        _solr = solr;
    }

    /// <summary>
    /// Adds or updates a product in the Solr index.
    /// </summary>
    public async Task AddOrUpdateProduct(Product product)
    {
        var solrProduct = new Product
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Quantity = product.Quantity,
            Gender = product.Gender,
            BrandId = product.BrandId,
            CategoryId = product.CategoryId,
            ColorId = product.ColorId,
            SizeId = product.SizeId,
        };

        await _solr.AddAsync(solrProduct);
        await _solr.CommitAsync();
    }

    /// <summary>
    /// Searches for products in Solr using a query string.
    /// Supports full-text search and filtering.
    /// </summary>
    public async Task<List<Product>> SearchProducts(
        string query, int? categoryId = null, int? brandId = null,
        int? colorId = null, int? sizeId = null, int? genderId = null,
        double? minPrice = null, double? maxPrice = null,
        bool fuzzy = false)
    {
        string solrQueryString = fuzzy ? $"{query}~" : query;
        var solrQuery = new SolrQuery(solrQueryString);
        var queryOptions = new QueryOptions
        {
            FilterQueries = new List<ISolrQuery>(),
            Rows = 10
        };

        if (categoryId.HasValue)
        {
            queryOptions.FilterQueries.Add(new SolrQueryByField("category_id", categoryId.Value.ToString()));
        }
        if (brandId.HasValue)
        {
            queryOptions.FilterQueries.Add(new SolrQueryByField("brand_id", brandId.Value.ToString()));
        }
        if (colorId.HasValue)
        {
            queryOptions.FilterQueries.Add(new SolrQueryByField("color_id", colorId.Value.ToString()));
        }
        if (sizeId.HasValue)
        {
            queryOptions.FilterQueries.Add(new SolrQueryByField("size_id", sizeId.Value.ToString()));
        }
        if (genderId.HasValue)
        {
            queryOptions.FilterQueries.Add(new SolrQueryByField("gender_id", genderId.Value.ToString()));
        }
        if (minPrice.HasValue || maxPrice.HasValue)
        {
            double min = minPrice ?? 0;
            double max = maxPrice ?? double.MaxValue;
            queryOptions.FilterQueries.Add(new SolrQueryByRange<double>("price", min, max));
        }

        var results = await _solr.QueryAsync(solrQuery, queryOptions);
        return results;
    }
}
