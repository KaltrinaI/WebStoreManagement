using Microsoft.EntityFrameworkCore;
using WebStoreApp.Data;
using WebStoreApp.DTOs;
using WebStoreApp.Models;
using WebStoreApp.Repositories.Interfaces;

namespace WebStoreApp.Repositories.Implementations
{
    public class ReportRepository : IReportRepository
    {
        private readonly AppDbContext _context;

        public ReportRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<double> GetDailyEarnings()
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
            await SaveReportMetadata(earnings, topSellingProductId,-1,-1);

            return earnings;
        }

        public async Task<double> GetMonthlyEarnings(int month, int year)
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

            var reportDate = DateTime.UtcNow; 

            var topProducts = await GetTopSellingProducts(1);  
            var topSellingProductId = topProducts.FirstOrDefault()?.ProductId ?? 0;
            await SaveReportMetadata(earnings, topSellingProductId, month, year);

            return earnings;
        }

        public async Task<double> GetTotalEarnings()
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
        public async Task<IEnumerable<ProductPerformanceReport>> GetTopSellingProducts(int topCount)
        {
            return await _context.OrderItems
                .Where(oi => oi.Order.OrderStatus == OrderStatus.Completed)
                .GroupBy(oi => oi.ProductId) 
                .Select(g => new ProductPerformanceReport
                {
                    ProductId = g.Key,
                    ProductName = g.FirstOrDefault().Product.Name,
                    TotalSales = g.Sum(oi => oi.Quantity * (
                        oi.Product.DiscountedPrice > 0 ?
                        oi.Product.DiscountedPrice :
                        oi.UnitPrice)),
                    UnitsSold = g.Sum(oi => oi.Quantity) 
                })
                .OrderByDescending(r => r.TotalSales)
                .Take(topCount)  
                .ToListAsync();
        }

        private async Task SaveReportMetadata(double totalEarnings, int topSellingProductId, int month, int year)
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


        public async Task<IEnumerable<Report>> GetAllReports()
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
    }
}
