using AutoMapper;
using WebStoreApp.DTOs;
using WebStoreApp.Models;
using WebStoreApp.Repositories.Interfaces;
using WebStoreApp.Services.Interfaces;

namespace WebStoreApp.Services.Implementations
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepository;
        private readonly IMapper _mapper;

        public ReportService(IReportRepository reportRepository, IMapper mapper)
        {
            _reportRepository = reportRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ReportDTO>> GetAllReports()
        {
            var reports = await _reportRepository.GetAllReports();
            return _mapper.Map<IEnumerable<ReportDTO>>(reports);
        }

        public  Task<double> GetDailyEarnings()
        {
            return _reportRepository.GetDailyEarnings();
        }

        public async Task<double> GetMonthlyEarnings(int month, int year)
        {
            return await _reportRepository.GetMonthlyEarnings(month, year);
        }

        public async Task<double> GetTotalEarnings()
        {
            return await _reportRepository.GetTotalEarnings();
        }

    }
}
