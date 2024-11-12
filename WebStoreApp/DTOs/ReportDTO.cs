using WebStoreApp.Models;

namespace WebStoreApp.DTOs
{
    public class ReportDTO
    {
        public DateTime ReportDate { get; set; }
        public double TotalEarnings { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public ProductResponseDTO TopSellingProduct { get; set; }
    }
}
