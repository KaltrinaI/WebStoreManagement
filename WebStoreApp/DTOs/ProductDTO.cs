

using WebStoreApp.Models;

namespace WebStoreApp.DTOs
{
    public class ProductDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public double DiscountedPrice {  get; set; }
        public int Quantity { get; set; } 
        public string CategoryName { get; set; }
        public string BrandName { get; set; }
        public string GenderName { get; set; }
        public string ColorName { get; set; }
        public string SizeName { get; set; }
        public List<Link> Links { get; set; } = new List<Link>();

    }
}
