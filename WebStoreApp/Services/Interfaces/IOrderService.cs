using WebStoreApp.DTOs;
using WebStoreApp.Models;

namespace WebStoreApp.Services.Interfaces
{
    public interface IOrderService
    {
        Task<OrderDTO> PlaceOrder(OrderRequestDTO order);
        Task<OrderResponseDTO> GetOrderById(int orderId);
        Task<IEnumerable<OrderResponseDTO>> GetOrdersByUserName(string userId);
        Task<IEnumerable<OrderResponseDTO>> GetAllOrders();
        Task UpdateOrderStatus(int orderId, OrderStatus status);
        Task<IEnumerable<OrderResponseDTO>> GetOrdersByStatus(OrderStatus status);
        Task AddOrderItem(int orderId, OrderItemDTO orderItem);
        Task RemoveOrderItem(int orderId, int orderItemId);
        Task CancelOrder(int orderId);
    }
}
