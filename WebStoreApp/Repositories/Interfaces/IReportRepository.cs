using WebStoreApp.DTOs;
using WebStoreApp.Models;

namespace WebStoreApp.Repositories.Interfaces
{
    public interface IReportRepository
    {
        Task<double> GetDailyEarnings();
        Task<double> GetMonthlyEarnings(int month, int year);
        Task<double> GetTotalEarnings();
        Task<IEnumerable<ProductPerformanceReport>> GetTopSellingProducts(int topCount);
        Task<IEnumerable<Report>> GetAllReports();
    }
}
