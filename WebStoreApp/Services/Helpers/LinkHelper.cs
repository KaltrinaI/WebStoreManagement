using WebStoreApp.Models;

namespace WebStoreApp.Services.Helpers
{
    public class LinkHelper
    {
        private readonly LinkGenerator _linkGenerator;

        public LinkHelper(LinkGenerator linkGenerator)
        {
            _linkGenerator = linkGenerator;
        }

        public List<Link> GenerateProductLinksForAllProducts(HttpContext httpContext, int productId, string category, IEnumerable<string> userRoles)
        {
            if (httpContext == null)
            {
                Console.WriteLine("HttpContext is null in GenerateProductLinks");
                throw new InvalidOperationException("HttpContext is null.");
            }

            string link = null;
            try
            {
                link = _linkGenerator.GetUriByAction(
            action: "GetProductById",
            controller: "Product",
            values: new { productId = productId },
            httpContext: httpContext
        );
            }
            catch (Exception ex) { }

            var links = new List<Link>
        {
            new Link(
                href: link,
                rel: "self",
                method: "GET"
            )
        };

            // Add link for viewing products in the same category if the user is a Customer
            if (userRoles.Contains("Customer"))
            {
                links.Add(new Link(
                href: _linkGenerator.GetUriByAction(httpContext, "CreateOrder", "Order"),
                rel: "create-order",
                method: "POST"
            ));
            }

            Console.WriteLine($"Links generated for product ID {productId}: {links.Count}");
            return links;
        }

        public List<Link> GenerateProductLinksForSingleProduct(HttpContext httpContext, int productId, string category, string brand, string gender, string size,
            string color, IEnumerable<string> userRoles)
        {
            if (httpContext == null)
                throw new InvalidOperationException("HttpContext is null.");

            var links = new List<Link>
        {
            new Link(
                href: _linkGenerator.GetUriByAction(httpContext, "GetProductById", "Product"),
                rel: "self",
                method: "GET"
            ),
            new Link(
                href: _linkGenerator.GetUriByAction(httpContext, "GetAllProducts", "Product"),
                rel: "all-products",
                method: "GET"
            )
        };

            if (userRoles.Contains("Admin") || userRoles.Contains("Advanced User"))
            {
                links.Add(new Link(
                    href: _linkGenerator.GetUriByAction(httpContext, "DeleteProduct", "Product", new { id = productId }),
                    rel: "delete",
                    method: "DELETE"
                ));

                links.Add(new Link(
                href: _linkGenerator.GetUriByAction(httpContext, "UpdateProduct", "Product", new { id = productId }),
                rel: "update",
                method: "PUT"
                ));
            }

            if (userRoles.Contains("Customer"))
            {
                links.Add(new Link(
                    href: _linkGenerator.GetUriByAction(httpContext, "GetProductsByCategory", "Product", new { category }),
                    rel: "category-products",
                    method: "GET"
                ));

                links.Add(new Link(
                    href: _linkGenerator.GetUriByAction(httpContext, "GetProductsByBrand", "Product", new { brand }),
                    rel: "brand-products",
                    method: "GET"
                ));

                links.Add(new Link(
                    href: _linkGenerator.GetUriByAction(httpContext, "GetProductsByGender", "Product", new { gender }),
                    rel: "gender-products",
                    method: "GET"
                ));

                links.Add(new Link(
                    href: _linkGenerator.GetUriByAction(httpContext, "GetProductsBySize", "Product", new { size }),
                    rel: "size-products",
                    method: "GET"
                ));

                links.Add(new Link(
                    href: _linkGenerator.GetUriByAction(httpContext, "GetProductsByColor", "Product", new { color }),
                    rel: "color-products",
                    method: "GET"
                ));
            }

            return links;
        }
    }
}
