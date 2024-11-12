using WebStoreApp.Models;

namespace WebStoreApp.DTOs
{
    public class OrderRequestDTO
    {
        public DateTime OrderDate { get; set; }
        public string Email { get; set; }
        public ICollection<OrderItemDTO> OrderItems { get; set; }

    }
}
