using AutoMapper;
using WebStoreApp.DTOs;
using WebStoreApp.Models;
using WebStoreApp.Repositories.Interfaces;
using WebStoreApp.Services.Interfaces;

namespace WebStoreApp.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public OrderService(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        public async Task<OrderDTO> PlaceOrder(OrderRequestDTO order)
        {
            var newOrder = _mapper.Map<Order>(order);
            var placedOrder = await _orderRepository.PlaceOrder(newOrder);

            return _mapper.Map<OrderDTO>(placedOrder);
        }

        public async Task<OrderResponseDTO> GetOrderById(int orderId)
        {
            var order = await _orderRepository.GetOrderById(orderId);
            return _mapper.Map<OrderResponseDTO>(order);
        }

        public async Task<IEnumerable<OrderResponseDTO>> GetOrdersByUserName(string userId)
        {
            var orders = await _orderRepository.GetOrdersByUserName(userId);
            return _mapper.Map<IEnumerable<OrderResponseDTO>>(orders);
        }

        public async Task<IEnumerable<OrderResponseDTO>> GetAllOrders()
        {
            var orders = await _orderRepository.GetAllOrders();
            return _mapper.Map<IEnumerable<OrderResponseDTO>>(orders);
        }

        public async Task UpdateOrderStatus(int orderId, OrderStatus status)
        {
            await _orderRepository.UpdateOrderStatus(orderId, status);
        }

        public async Task<IEnumerable<OrderResponseDTO>> GetOrdersByStatus(OrderStatus status)
        {
            var orders = await _orderRepository.GetOrdersByStatus(status);
            return _mapper.Map<IEnumerable<OrderResponseDTO>>(orders);
        }

        public async Task AddOrderItem(int orderId, OrderItemDTO orderItemDto)
        {
            var orderItem = _mapper.Map<OrderItem>(orderItemDto);
            await _orderRepository.AddOrderItem(orderId, orderItem);
        }

        public async Task RemoveOrderItem(int orderId, int orderItemId)
        {
            await _orderRepository.RemoveOrderItem(orderId,orderItemId);
        }
        
        public async Task CancelOrder(int orderId)
        {
            await _orderRepository.CancelOrder(orderId);
        }
    }
}
