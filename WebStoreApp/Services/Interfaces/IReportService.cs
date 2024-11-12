using WebStoreApp.DTOs;
using WebStoreApp.Models;

namespace WebStoreApp.Services.Interfaces
{
    public interface IReportService
    {
        Task<double> GetDailyEarnings();
        Task<double> GetMonthlyEarnings(int month, int year);
        Task<double> GetTotalEarnings();
        Task<IEnumerable<ReportDTO>> GetAllReports();
    }
}
