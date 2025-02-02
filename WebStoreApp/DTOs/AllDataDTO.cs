using System.Collections.Generic;

namespace WebStoreApp.DTOs
{
    public class AllDataDTO
    {
        public IEnumerable<ProductDTO> Products { get; set; }
        public IEnumerable<OrderResponseDTO> Orders { get; set; }
        public double TotalEarnings { get; set; }
        public double DailyEarnings { get; set; }
        public double MonthlyEarnings { get; set; }
    }
}
