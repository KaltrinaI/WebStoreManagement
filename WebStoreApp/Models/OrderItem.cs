using System.Text.Json.Serialization;

namespace WebStoreApp.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int OrderId { get; set; }
        [JsonIgnore]
        public Order Order { get; set; }
    }
}
