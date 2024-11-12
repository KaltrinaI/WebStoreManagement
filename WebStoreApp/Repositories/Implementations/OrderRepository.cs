using Microsoft.EntityFrameworkCore;
using WebStoreApp.Data;
using WebStoreApp.Models;
using WebStoreApp.Repositories.Interfaces;

namespace WebStoreApp.Repositories.Implementations
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddOrderItem(int orderId, OrderItem orderItem)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems) 
                .SingleOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                throw new InvalidOperationException("Order not found");
            }

            var product = await _context.Products.FindAsync(orderItem.ProductId);
            if (product == null || product.Quantity < orderItem.Quantity)
            {
                throw new InvalidOperationException("Insufficient stock for product");
            }

            if (order.OrderItems == null)
            {
                order.OrderItems = new List<OrderItem>();
            }

            product.Quantity -= orderItem.Quantity;
            order.OrderItems.Add(orderItem);

            await _context.SaveChangesAsync();
        }


        public async Task<IEnumerable<Order>> GetAllOrders()
        {
            return await _context.Orders
                .Include(o => o.User) 
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .ThenInclude(p => p.Category) 
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
        public async Task<Order> GetOrderById(int orderId)
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

            return order ?? new Order();
        }


        public async Task<IEnumerable<Order>> GetOrdersByStatus(OrderStatus status)
        {
            return await _context.Orders
                .Where(o => o.OrderStatus == status)
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


        public async Task<IEnumerable<Order>> GetOrdersByUserName(string username)
        {
            return await _context.Orders
                .Where(o => o.User.Email == username)
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

        public async Task<Order> PlaceOrder(Order order)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == order.User.Email);

                if (user == null)
                {
                    throw new InvalidOperationException("User with this email does not exist.");
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
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task RemoveOrderItem(int orderId, int orderItemId)
        {
            var orderItem = await _context.OrderItems
                .FirstOrDefaultAsync(oi => oi.Id == orderItemId && oi.OrderId == orderId);

            if (orderItem == null)
            {
                throw new InvalidOperationException("Order item not found in the specified order");
            }

            var product = await _context.Products.FindAsync(orderItem.ProductId);
            if (product != null)
            {
                product.Quantity += orderItem.Quantity;
            }

            _context.OrderItems.Remove(orderItem);
            await _context.SaveChangesAsync();
        }


        public async Task UpdateOrderStatus(int orderId, OrderStatus status)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                throw new InvalidOperationException("Order not found");
            }

            order.OrderStatus = status;
            await _context.SaveChangesAsync();
        }

        public async Task CancelOrder(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                throw new InvalidOperationException("Order not found");
            }

            if (order.OrderStatus == OrderStatus.Canceled)
            {
                throw new InvalidOperationException("Order is already canceled");
            }

            order.OrderStatus = OrderStatus.Canceled;

            await _context.SaveChangesAsync();
        }

        public async Task<int> GetTotalQuantitySold(int productId)
        {
            return await _context.OrderItems
                .Where(oi => oi.ProductId == productId && oi.Order.OrderStatus == OrderStatus.Completed)
                .SumAsync(oi => oi.Quantity);
        }

    }
}
