using SolrNet.Attributes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Reflection;

namespace WebStoreApp.Models
{
    public class Product
    {
        [Key]
        [SolrUniqueKey("id")]
        public int Id { get; set; }

        [SolrField("name")]
        public string Name { get; set; }

        [SolrField("description")]
        public string Description { get; set; }

        [SolrField("price")]
        public double Price { get; set; }

        [SolrField("discounted_price")]
        public double DiscountedPrice { get; set; } = 0;

        [SolrField("quantity")]
        public int Quantity { get; set; }


        [SolrField("category_id")]
        public int CategoryId { get; set; }


        [SolrField("brand_id")]
        public int BrandId { get; set; }

        [SolrField("gender_id")]
        public int GenderId { get; set; }

        [SolrField("color_id")]
        public int ColorId { get; set; }

        [SolrField("size_id")]
        public int SizeId { get; set; }

        public bool InStock => Quantity > 0;

        public Category Category { get; set; }
        public Brand Brand { get; set; }
        public Gender Gender { get; set; }
        public Color Color { get; set; }
        public Size Size { get; set; }
        public ICollection<Discount>? Discounts { get; set; } = new List<Discount>();

    }
}
