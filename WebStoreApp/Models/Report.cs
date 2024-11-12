namespace WebStoreApp.Models
{
    public class Report
    {
        public int Id { get; set; }
        public DateTime ReportDate { get; set; }
        public double TotalEarnings { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
        public int TopSellingProductId { get; set; }
        public Product TopSellingProduct { get; set; }
    }
}
