namespace WebStoreApp.DTOs
{
    public class ProductResponseDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public double DiscountedPrice { get; set; }
        public string CategoryName { get; set; }
        public string BrandName { get; set; }
        public string GenderName { get; set; }
        public string ColorName { get; set; }
        public string SizeName { get; set; }
    }
}
