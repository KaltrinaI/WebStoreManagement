using System.ComponentModel;
using System.Drawing;
using System.Reflection;

namespace WebStoreApp.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public double DiscountedPrice { get; set; } = 0;
        public int Quantity { get; set; } 
        public bool InStock => Quantity > 0;

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public int BrandId { get; set; }
        public Brand Brand { get; set; }

        public int GenderId { get; set; }
        public Gender Gender { get; set; }

        public int ColorId { get; set; }
        public Color Color { get; set; }

        public int SizeId { get; set; }
        public Size Size { get; set; }

        public ICollection<Discount>? Discounts { get; set; } = new List<Discount>();

    }
}
