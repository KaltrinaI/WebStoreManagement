using HotChocolate;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using System.Fabric.Query;
using WebStoreApp.DTOs;
using WebStoreApp.Services.Interfaces;

namespace WebStoreApp.GraphQL
{
    public class CombinedQuery
    {
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        private readonly IReportService _reportService;

        public CombinedQuery(IProductService productService, IOrderService orderService, IReportService reportService)
        {
            _productService = productService;
            _orderService = orderService;
            _reportService = reportService;
        }

        [GraphQLName("getAllData")]
        public async Task<AllDataDTO> GetAllData( IResolverContext context)
        {
            try
            {
                // Run tasks in parallel to improve performance
                var (products, orders, totalEarnings, dailyEarnings, monthlyEarnings) = await FetchDataAsync(context.RequestAborted);
               
                return new AllDataDTO
                {
                    Products = products,
                    Orders = orders,
                    TotalEarnings = totalEarnings,
                    DailyEarnings = dailyEarnings,
                    MonthlyEarnings = monthlyEarnings
                };
            }
            catch (Exception ex)
            {
                context.ReportError(
                    ErrorBuilder.New()
                        .SetMessage("An error occurred while fetching data.")
                        .SetCode("FETCH_ERROR")
                        .SetException(ex)
                        .Build()
                );

                return null; // GraphQL will handle the error
            }
        }

        private async Task<(IEnumerable<ProductDTO>, IEnumerable<OrderResponseDTO>, double, double, double)> FetchDataAsync(CancellationToken cancellationToken)
        {
            var productsTask =  await _productService.GetAllProducts();
            var ordersTask =await  _orderService.GetAllOrders();
            var totalEarningsTask =await  _reportService.GetTotalEarnings();
            var dailyEarningsTask =await  _reportService.GetDailyEarnings();
            var monthlyEarningsTask =await _reportService.GetMonthlyEarnings(DateTime.UtcNow.Month, DateTime.UtcNow.Year);


            return (
                 productsTask,
                 ordersTask,
                 totalEarningsTask,
                 dailyEarningsTask,
                 monthlyEarningsTask
            );
        }
    }
}
