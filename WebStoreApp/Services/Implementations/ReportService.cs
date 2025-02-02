using AutoMapper;
using Microsoft.Extensions.Logging;
using WebStoreApp.DTOs;
using WebStoreApp.Exceptions;
using WebStoreApp.Repositories.Interfaces;
using WebStoreApp.Services.Interfaces;

namespace WebStoreApp.Services.Implementations
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ReportService> _logger;

        public ReportService(IReportRepository reportRepository, IMapper mapper, ILogger<ReportService> logger)
        {
            _reportRepository = reportRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<ReportDTO>> GetAllReports()
        {
            try
            {
                var reports = await _reportRepository.GetAllReports();
                return _mapper.Map<IEnumerable<ReportDTO>>(reports);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while retrieving all reports.");
                throw new ServiceException("An unexpected error occurred while retrieving all reports.", ex);
            }
        }

        public async Task<double> GetDailyEarnings()
        {
            try
            {
                return await _reportRepository.GetDailyEarnings();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while calculating daily earnings.");
                throw new ServiceException("An unexpected error occurred while calculating daily earnings.", ex);
            }
        }

        public async Task<double> GetMonthlyEarnings(int month, int year)
        {
            try
            {
                if (month < 1 || month > 12)
                {
                    _logger.LogWarning("Invalid month provided: {Month}.", month);
                    throw new ServiceException("Invalid month. Please provide a value between 1 and 12.");
                }

                if (year < 1)
                {
                    _logger.LogWarning("Invalid year provided: {Year}.", year);
                    throw new ServiceException("Invalid year. Please provide a positive year value.");
                }

                return await _reportRepository.GetMonthlyEarnings(month, year);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while calculating monthly earnings for {Month}/{Year}.", month, year);
                throw new ServiceException("An unexpected error occurred while calculating monthly earnings.", ex);
            }
        }

        public async Task<double> GetTotalEarnings()
        {
            try
            {
                return await _reportRepository.GetTotalEarnings();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while calculating total earnings.");
                throw new ServiceException("An unexpected error occurred while calculating total earnings.", ex);
            }
        }
    }
}
