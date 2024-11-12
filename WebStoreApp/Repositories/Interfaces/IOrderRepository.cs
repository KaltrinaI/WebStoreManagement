using WebStoreApp.Models;

namespace WebStoreApp.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order> PlaceOrder(Order order); 
        Task<Order> GetOrderById(int orderId); 
        Task<IEnumerable<Order>> GetOrdersByUserName(string username); 
        Task<IEnumerable<Order>> GetAllOrders(); 
        Task UpdateOrderStatus(int orderId, OrderStatus status);
        Task<IEnumerable<Order>> GetOrdersByStatus(OrderStatus status); 
        Task AddOrderItem(int orderId, OrderItem orderItem); 
        Task RemoveOrderItem(int orderId, int orderItemId); 
        Task CancelOrder(int orderId);
        Task<int> GetTotalQuantitySold(int productId);


    }
}
