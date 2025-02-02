using HotChocolate.Subscriptions;
using WebStoreApp.DTOs;
using WebStoreApp.Services.Interfaces;
using HotChocolate;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using HotChocolate.Resolvers;
using WebStoreApp.Exceptions;

namespace WebStoreApp.GraphQL.Mutations
{
    public class Mutation
    {
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        private readonly IMemoryCache _cache;
        private readonly ILogger<Mutation> _logger;

        public Mutation(
            IProductService productService,
            IOrderService orderService,
            IMemoryCache cache,
            ILogger<Mutation> logger)
        {
            _productService = productService;
            _orderService = orderService;
            _cache = cache;
            _logger = logger;
        }

        [GraphQLName("updateProductStock")]
        public async Task<ProductDTO> UpdateProductStock(int productId, int newStock,
            [Service] ITopicEventSender eventSender, IResolverContext context)
        {
            try
            {
                if (productId <= 0 || newStock < 0)
                {
                    throw new ArgumentException("Invalid product ID or stock quantity.");
                }

                await _productService.UpdateProductStock(productId, newStock);
                var updatedProduct = await _productService.GetProductById(productId);

                await eventSender.SendAsync("ProductStockUpdated", updatedProduct);

                return updatedProduct;
            }
            catch (ServiceException ex)
            {
                context.ReportError(
                    ErrorBuilder.New()
                        .SetMessage(ex.Message)
                        .SetCode("SERVICE_ERROR")
                        .Build()
                );
                return null;
            }
            catch (Exception ex)
            {
                context.ReportError(
                    ErrorBuilder.New()
                        .SetMessage("An unexpected error occurred while updating product stock.")
                        .SetCode("UNEXPECTED_ERROR")
                        .SetException(ex)
                        .Build()
                );
                return null;
            }
        }
    }
}
