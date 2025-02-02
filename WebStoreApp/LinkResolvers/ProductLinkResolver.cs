
using AutoMapper;
using WebStoreApp.DTOs;
using WebStoreApp.Models;
using WebStoreApp.Services.Helpers;

namespace WebStoreApp.LinkResolvers
{
    public class ProductLinksResolver : IValueResolver<Product, ProductDTO, List<Link>>
    {
        private readonly LinkHelper _linkHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductLinksResolver(LinkHelper linkHelper, IHttpContextAccessor httpContextAccessor)
        {
            _linkHelper = linkHelper;
            _httpContextAccessor = httpContextAccessor;
        }

        public List<Link> Resolve(Product source, ProductDTO destination, List<Link> destMember, ResolutionContext context)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext == null)
            {
                throw new InvalidOperationException("HttpContext is null.");
            }

            // Extract user roles
            var userRoles = httpContext.User.FindAll(System.Security.Claims.ClaimTypes.Role)
                                .Select(role => role.Value)
                                .ToList();

            // Determine which method to call based on context
            if (context.Items.TryGetValue("IsSingleProduct", out var isSingleProduct) && isSingleProduct is bool singleProduct && singleProduct)
            {
                // Single Product Context
                return _linkHelper.GenerateProductLinksForSingleProduct(
                    httpContext,
                    source.Id,
                    source.Category.Name,
                    source.Brand.Name,
                    source.Gender.Name,
                    source.Size.Name,
                    source.Color.Name,
                    userRoles
                );
            }

            // All Products Context
            return _linkHelper.GenerateProductLinksForAllProducts(
                httpContext,
                source.Id,
                source.Category.Name,
                userRoles
            );
        }

    }
}
