namespace WebStoreApp.DTOs
{
    public class ProductPerformanceReport
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public double TotalSales { get; set; }
        public int UnitsSold { get; set; }
    }
}
