using Microsoft.EntityFrameworkCore;
using WebStoreApp.Data;
using WebStoreApp.DTOs;
using WebStoreApp.Models;
using WebStoreApp.Repositories.Interfaces;
using WebStoreApp.Exceptions;
using Microsoft.Extensions.Logging;

namespace WebStoreApp.Repositories.Implementations
{
    public class ReportRepository : IReportRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ReportRepository> _logger;

        public ReportRepository(AppDbContext context, ILogger<ReportRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<double> GetDailyEarnings()
        {
            try
            {
                DateTime targetDate = DateTime.UtcNow.Date;

                var orders = await _context.Orders
                    .Where(o => o.OrderDate.Date >= targetDate && o.OrderDate < targetDate.AddDays(1) && o.OrderStatus == OrderStatus.Completed)
                    .Include(o => o.OrderItems)
                    .ToListAsync();

                var productIds = orders.SelectMany(o => o.OrderItems)
                                       .Select(oi => oi.ProductId)
                                       .Distinct()
                                       .ToList();

                var products = await _context.Products
                    .Where(p => productIds.Contains(p.Id))
                    .ToListAsync();

                var earnings = orders
                    .SelectMany(o => o.OrderItems)
                    .Sum(oi =>
                    {
                        var product = products.FirstOrDefault(p => p.Id == oi.ProductId);
                        if (product != null)
                        {
                            var unitPrice = product.DiscountedPrice > 0 ? product.DiscountedPrice : product.Price;
                            return oi.Quantity * unitPrice;
                        }
                        return 0;
                    });

                var topProducts = await GetTopSellingProducts(1);
                var topSellingProductId = topProducts.FirstOrDefault()?.ProductId ?? 0;

                await SaveReportMetadata(earnings, topSellingProductId, -1, -1);

                return earnings;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while calculating daily earnings.");
                throw new RepositoryException("Error occurred while calculating daily earnings.", ex);
            }
        }

        public async Task<double> GetMonthlyEarnings(int month, int year)
        {
            try
            {
                var startDate = new DateTime(year, month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);

                var orders = await _context.Orders
                    .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate && o.OrderStatus == OrderStatus.Completed)
                    .Include(o => o.OrderItems)
                    .ToListAsync();

                var productIds = orders.SelectMany(o => o.OrderItems)
                                       .Select(oi => oi.ProductId)
                                       .Distinct()
                                       .ToList();

                var products = await _context.Products
                    .Where(p => productIds.Contains(p.Id))
                    .ToListAsync();

                var earnings = orders
                    .SelectMany(o => o.OrderItems)
                    .Sum(oi =>
                    {
                        var product = products.FirstOrDefault(p => p.Id == oi.ProductId);
                        if (product != null)
                        {
                            var unitPrice = product.DiscountedPrice > 0 ? product.DiscountedPrice : product.Price;
                            return oi.Quantity * unitPrice;
                        }
                        return 0;
                    });

                var topProducts = await GetTopSellingProducts(1);
                var topSellingProductId = topProducts.FirstOrDefault()?.ProductId ?? 0;

                await SaveReportMetadata(earnings, topSellingProductId, month, year);

                return earnings;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while calculating monthly earnings for {Month}/{Year}.", month, year);
                throw new RepositoryException("Error occurred while calculating monthly earnings.", ex);
            }
        }

        public async Task<double> GetTotalEarnings()
        {
            try
            {
                var completedOrders = await _context.Orders
                    .Where(o => o.OrderStatus == OrderStatus.Completed)
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .ToListAsync();

                var earnings = completedOrders
                    .SelectMany(o => o.OrderItems)
                    .Sum(oi =>
                    {
                        var product = oi.Product;
                        if (product != null)
                        {
                            var unitPrice = product.DiscountedPrice > 0 ? product.DiscountedPrice : product.Price;
                            return oi.Quantity * unitPrice;
                        }
                        return 0;
                    });

                return earnings;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while calculating total earnings.");
                throw new RepositoryException("Error occurred while calculating total earnings.", ex);
            }
        }

        public async Task<IEnumerable<ProductPerformanceReport>> GetTopSellingProducts(int topCount)
        {
            try
            {
                return await _context.OrderItems
                    .Where(oi => oi.Order.OrderStatus == OrderStatus.Completed)
                    .GroupBy(oi => oi.ProductId)
                    .Select(g => new ProductPerformanceReport
                    {
                        ProductId = g.Key,
                        ProductName = g.FirstOrDefault().Product.Name,
                        TotalSales = g.Sum(oi => oi.Quantity * (
                            oi.Product.DiscountedPrice > 0 ? oi.Product.DiscountedPrice : oi.UnitPrice)),
                        UnitsSold = g.Sum(oi => oi.Quantity)
                    })
                    .OrderByDescending(r => r.TotalSales)
                    .Take(topCount)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving top-selling products.");
                throw new RepositoryException("Error occurred while retrieving top-selling products.", ex);
            }
        }

        private async Task SaveReportMetadata(double totalEarnings, int topSellingProductId, int month, int year)
        {
            try
            {
                var report = new Report
                {
                    ReportDate = DateTime.UtcNow,
                    TotalEarnings = totalEarnings,
                    Month = month,
                    Year = year,
                    TopSellingProductId = topSellingProductId,
                };

                await _context.Reports.AddAsync(report);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error occurred while saving report metadata.");
                throw new RepositoryException("Error occurred while saving report metadata.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while saving report metadata.");
                throw new RepositoryException("An unexpected error occurred while saving report metadata.", ex);
            }
        }

        public async Task<IEnumerable<Report>> GetAllReports()
        {
            try
            {
                return await _context.Reports
                    .Include(r => r.TopSellingProduct)
                    .ThenInclude(p => p.Category)
                    .Include(r => r.TopSellingProduct)
                    .ThenInclude(p => p.Brand)
                    .Include(r => r.TopSellingProduct)
                    .ThenInclude(p => p.Gender)
                    .Include(r => r.TopSellingProduct)
                    .ThenInclude(p => p.Color)
                    .Include(r => r.TopSellingProduct)
                    .ThenInclude(p => p.Size)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all reports.");
                throw new RepositoryException("Error occurred while retrieving all reports.", ex);
            }
        }
    }
}
