using WebStoreApp.Models;

namespace WebStoreApp.DTOs
{
    public class OrderDTO
    {
        public OrderStatus OrderStatus { get; set; }
        public DateTime OrderDate { get; set; }
        public UserDTO User { get; set; }
        public ICollection<OrderItemDTO> OrderItems { get; set; }
    }
}
