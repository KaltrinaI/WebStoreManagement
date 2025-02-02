using AutoMapper;
using WebStoreApp.DTOs;
using WebStoreApp.Models;
using WebStoreApp.Repositories.Interfaces;
using WebStoreApp.Services.Interfaces;
using WebStoreApp.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;

namespace WebStoreApp.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderService> _logger;

        public OrderService(IOrderRepository orderRepository, IMapper mapper, ILogger<OrderService> logger)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<OrderDTO> PlaceOrder(OrderRequestDTO order)
        {
            try
            {
                if (order == null)
                {
                    _logger.LogWarning("Order data is null.");
                    throw new ServiceException("Order data cannot be null.");
                }

                var newOrder = _mapper.Map<Order>(order);
                var placedOrder = await _orderRepository.PlaceOrder(newOrder);

                return _mapper.Map<OrderDTO>(placedOrder);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while placing an order.");
                throw new ServiceException("An unexpected error occurred while placing the order.", ex);
            }
        }


        public async Task<OrderResponseDTO> GetOrderById(int orderId)
        {
            try
            {
                if (orderId <= 0)
                {
                    _logger.LogWarning("Invalid Order ID: {OrderId}", orderId);
                    throw new ServiceException("Invalid Order ID.");
                }

                var order = await _orderRepository.GetOrderById(orderId);
                if (order == null)
                {
                    _logger.LogWarning("Order with ID {OrderId} not found.", orderId);
                    throw new ServiceException($"Order with ID {orderId} not found.");
                }

                return _mapper.Map<OrderResponseDTO>(order);
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving order by ID: {OrderId}", orderId);
                throw new ServiceException("An error occurred while retrieving the order.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while retrieving order by ID: {OrderId}", orderId);
                throw new ServiceException("An unexpected error occurred while retrieving the order.", ex);
            }
        }

        public async Task<IEnumerable<OrderResponseDTO>> GetOrdersByUserName(string userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    _logger.LogWarning("User ID is null or empty.");
                    throw new ServiceException("User ID cannot be null or empty.");
                }

                var orders = await _orderRepository.GetOrdersByUserName(userId);
                return _mapper.Map<IEnumerable<OrderResponseDTO>>(orders);
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving orders for user ID: {UserId}", userId);
                throw new ServiceException("An error occurred while retrieving orders for the user.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while retrieving orders for user ID: {UserId}", userId);
                throw new ServiceException("An unexpected error occurred while retrieving orders for the user.", ex);
            }
        }

        public async Task<IEnumerable<OrderResponseDTO>> GetAllOrders()
        {
            try
            {
                var orders = await _orderRepository.GetAllOrders();
                return _mapper.Map<IEnumerable<OrderResponseDTO>>(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while retrieving all orders.");
                throw new ServiceException("An unexpected error occurred while retrieving all orders.", ex);
            }
        }

        public async Task UpdateOrderStatus(int orderId, OrderStatus status)
        {
            try
            {
                if (orderId <= 0)
                {
                    _logger.LogWarning("Invalid Order ID: {OrderId}", orderId);
                    throw new ServiceException("Invalid Order ID.");
                }

                await _orderRepository.UpdateOrderStatus(orderId, status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while updating order status for ID: {OrderId}", orderId);
                throw new ServiceException("An unexpected error occurred while updating the order status.", ex);
            }
        }


        public async Task<IEnumerable<OrderResponseDTO>> GetOrdersByStatus(OrderStatus status)
        {
            try
            {
                var orders = await _orderRepository.GetOrdersByStatus(status);
                return _mapper.Map<IEnumerable<OrderResponseDTO>>(orders);
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving orders by status: {Status}", status);
                throw new ServiceException("An error occurred while retrieving orders by status.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while retrieving orders by status: {Status}", status);
                throw new ServiceException("An unexpected error occurred while retrieving orders by status.", ex);
            }
        }

        public async Task AddOrderItem(int orderId, OrderItemDTO orderItemDto)
        {
            try
            {
                if (orderItemDto == null)
                {
                    _logger.LogWarning("Order item data is null.");
                    throw new ServiceException("Order item data cannot be null.");
                }

                var orderItem = _mapper.Map<OrderItem>(orderItemDto);
                await _orderRepository.AddOrderItem(orderId, orderItem);
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, "Error occurred while adding order item to order ID: {OrderId}", orderId);
                throw new ServiceException("An error occurred while adding an order item.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while adding order item to order ID: {OrderId}", orderId);
                throw new ServiceException("An unexpected error occurred while adding an order item.", ex);
            }
        }

        public async Task RemoveOrderItem(int orderId, int orderItemId)
        {
            try
            {
                if (orderId <= 0 || orderItemId <= 0)
                {
                    _logger.LogWarning("Invalid Order ID: {OrderId} or Order Item ID: {OrderItemId}", orderId, orderItemId);
                    throw new ServiceException("Order ID and Order Item ID must be valid positive integers.");
                }

                await _orderRepository.RemoveOrderItem(orderId, orderItemId);
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, "Error occurred while removing order item with ID {OrderItemId} from order ID {OrderId}.", orderItemId, orderId);
                throw new ServiceException("An error occurred while removing the order item.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while removing order item with ID {OrderItemId} from order ID {OrderId}.", orderItemId, orderId);
                throw new ServiceException("An unexpected error occurred while removing the order item.", ex);
            }
        }

        public async Task CancelOrder(int orderId)
        {
            try
            {
                if (orderId <= 0)
                {
                    _logger.LogWarning("Invalid Order ID: {OrderId}", orderId);
                    throw new ServiceException("Invalid Order ID.");
                }

                await _orderRepository.CancelOrder(orderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while canceling order with ID {OrderId}.", orderId);
                throw new ServiceException("An unexpected error occurred while canceling the order.", ex);
            }
        }

    }
}
