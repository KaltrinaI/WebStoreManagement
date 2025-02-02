using Microsoft.EntityFrameworkCore;
using WebStoreApp.Data;
using WebStoreApp.Models;
using WebStoreApp.Repositories.Interfaces;
using WebStoreApp.Exceptions; 
using Microsoft.Extensions.Logging;

namespace WebStoreApp.Repositories.Implementations
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<OrderRepository> _logger;

        public OrderRepository(AppDbContext context, ILogger<OrderRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task AddOrderItem(int orderId, OrderItem orderItem)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                    .SingleOrDefaultAsync(o => o.Id == orderId);

                if (order == null)
                {
                    _logger.LogWarning("Order with ID {OrderId} not found.", orderId);
                    throw new RepositoryException("Order not found.");
                }

                var product = await _context.Products.FindAsync(orderItem.ProductId);
                if (product == null || product.Quantity < orderItem.Quantity)
                {
                    _logger.LogWarning("Insufficient stock for product ID {ProductId}.", orderItem.ProductId);
                    throw new RepositoryException("Insufficient stock for product.");
                }

                product.Quantity -= orderItem.Quantity;

                if (order.OrderItems == null)
                {
                    order.OrderItems = new List<OrderItem>();
                }

                order.OrderItems.Add(orderItem);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error occurred while adding an order item to order ID {OrderId}.", orderId);
                throw new RepositoryException("Error occurred while adding an order item.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while adding an order item to order ID {OrderId}.", orderId);
                throw new RepositoryException("An unexpected error occurred while adding an order item.", ex);
            }
        }

        public async Task<IEnumerable<Order>> GetAllOrders()
        {
            try
            {
                return await _context.Orders
                    .Include(o => o.User)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product.Category)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product.Brand)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product.Gender)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product.Color)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product.Size)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all orders.");
                throw new RepositoryException("Error occurred while retrieving all orders.", ex);
            }
        }

        public async Task<Order> GetOrderById(int orderId)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.User)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product.Category)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product.Brand)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product.Gender)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product.Color)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product.Size)
                    .FirstOrDefaultAsync(o => o.Id == orderId);

                if (order == null)
                {
                    _logger.LogWarning("Order with ID {OrderId} not found.", orderId);
                    throw new RepositoryException($"Order with ID {orderId} not found.");
                }

                return order;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving order by ID {OrderId}.", orderId);
                throw new RepositoryException("Error occurred while retrieving the order by ID.", ex);
            }
        }

        public async Task<IEnumerable<Order>> GetOrdersByStatus(OrderStatus status)
        {
            try
            {
                return await _context.Orders
                    .Where(o => o.OrderStatus == status)
                    .Include(o => o.User)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving orders with status {Status}.", status);
                throw new RepositoryException("Error occurred while retrieving orders by status.", ex);
            }
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserName(string username)
        {
            try
            {
                return await _context.Orders
                    .Where(o => o.User.Email == username)
                    .Include(o => o.User)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving orders for username {Username}.", username);
                throw new RepositoryException("Error occurred while retrieving orders by username.", ex);
            }
        }

        public async Task<Order> PlaceOrder(Order order)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == order.User.Email);

                if (user == null)
                {
                    _logger.LogWarning("User with email {Email} not found.", order.User.Email);
                    throw new RepositoryException("User not found.");
                }

                order.UserId = user.Id;
                order.User = user;
                order.OrderStatus = OrderStatus.Pending;

                await _context.Orders.AddAsync(order);

                foreach (var orderItem in order.OrderItems)
                {
                    await _context.OrderItems.AddAsync(orderItem);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return order;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while placing the order.");
                await transaction.RollbackAsync();
                throw new RepositoryException("Error occurred while placing the order.", ex);
            }
        }

        public async Task RemoveOrderItem(int orderId, int orderItemId)
        {
            try
            {
                var orderItem = await _context.OrderItems
                    .FirstOrDefaultAsync(oi => oi.Id == orderItemId && oi.OrderId == orderId);

                if (orderItem == null)
                {
                    _logger.LogWarning("Order item with ID {OrderItemId} not found in order {OrderId}.", orderItemId, orderId);
                    throw new RepositoryException("Order item not found.");
                }

                var product = await _context.Products.FindAsync(orderItem.ProductId);
                if (product != null)
                {
                    product.Quantity += orderItem.Quantity;
                }

                _context.OrderItems.Remove(orderItem);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while removing order item with ID {OrderItemId} from order {OrderId}.", orderItemId, orderId);
                throw new RepositoryException("Error occurred while removing the order item.", ex);
            }
        }

        public async Task UpdateOrderStatus(int orderId, OrderStatus status)
        {
            try
            {
                var order = await _context.Orders.FindAsync(orderId);

                if (order == null)
                {
                    _logger.LogWarning("Order with ID {OrderId} not found.", orderId);
                    throw new RepositoryException("Order not found.");
                }

                order.OrderStatus = status;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating the status of order {OrderId}.", orderId);
                throw new RepositoryException("Error occurred while updating the order status.", ex);
            }
        }

        public async Task CancelOrder(int orderId)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                    .FirstOrDefaultAsync(o => o.Id == orderId);

                if (order == null)
                {
                    _logger.LogWarning("Order with ID {OrderId} not found.", orderId);
                    throw new RepositoryException("Order not found.");
                }

                if (order.OrderStatus == OrderStatus.Canceled)
                {
                    _logger.LogWarning("Order with ID {OrderId} is already canceled.", orderId);
                    throw new RepositoryException("Order is already canceled.");
                }

                order.OrderStatus = OrderStatus.Canceled;

                foreach (var orderItem in order.OrderItems)
                {
                    var product = await _context.Products.FindAsync(orderItem.ProductId);
                    if (product != null)
                    {
                        product.Quantity += orderItem.Quantity;
                    }
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while canceling order with ID {OrderId}.", orderId);
                throw new RepositoryException("Error occurred while canceling the order.", ex);
            }
        }

        public async Task<int> GetTotalQuantitySold(int productId)
        {
            try
            {
                return await _context.OrderItems
                    .Where(oi => oi.ProductId == productId && oi.Order.OrderStatus == OrderStatus.Completed)
                    .SumAsync(oi => oi.Quantity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while calculating total quantity sold for product ID {ProductId}.", productId);
                throw new RepositoryException("Error occurred while retrieving total quantity sold.", ex);
            }
        }
    }
}
