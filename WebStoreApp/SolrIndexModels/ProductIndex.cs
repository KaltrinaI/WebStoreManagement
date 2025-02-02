using SolrNet.Attributes;

namespace WebStoreApp.SolrIndexModels
{
    public class ProductIndex
    {
        [SolrUniqueKey("id")]
        public string Id { get; set; }

        [SolrField("name")]
        public string Name { get; set; }

        [SolrField("description")]
        public string Description { get; set; }

        [SolrField("price")]
        public double Price { get; set; }

        [SolrField("category")]
        public string Category { get; set; }

        [SolrField("brand")]
        public string Brand { get; set; }

        [SolrField("color")]
        public string Color { get; set; }

        [SolrField("size")]
        public string Size { get; set; }
    }
}
