using WebStoreApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebStoreApp.Services.Helpers
{
    public class LinkHelper
    {
        private readonly LinkGenerator _linkGenerator;

        public LinkHelper(LinkGenerator linkGenerator)
        {
            _linkGenerator = linkGenerator ?? throw new ArgumentNullException(nameof(linkGenerator));
        }

        public List<Link> GenerateProductLinksForAllProducts(HttpContext httpContext, int productId, string category, IEnumerable<string> userRoles)
        {
            if (httpContext == null)
            {
                Console.WriteLine("HttpContext is null in GenerateProductLinksForAllProducts");
                throw new InvalidOperationException("HttpContext is null.");
            }

            var links = new List<Link>();

            try
            {
                string productLink = _linkGenerator.GetUriByAction(
                    action: "GetProductById",
                    controller: "Product",
                    values: new { productId },
                    httpContext: httpContext
                );

                if (!string.IsNullOrEmpty(productLink))
                {
                    links.Add(new Link(
                        href: productLink,
                        rel: "self",
                        method: "GET"
                    ));
                }
                else
                {
                    Console.WriteLine($"Failed to generate link for GetProductById (Product ID: {productId})");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating link for GetProductById: {ex.Message}");
            }

            Console.WriteLine($"Links generated for product ID {productId}: {links.Count}");
            return links;
        }

        public List<Link> GenerateProductLinksForSingleProduct(
            HttpContext httpContext, int productId, string category, string brand, string gender, string size,
            string color, IEnumerable<string> userRoles)
        {
            if (httpContext == null)
            {
                Console.WriteLine("HttpContext is null in GenerateProductLinksForSingleProduct");
                throw new InvalidOperationException("HttpContext is null.");
            }

            var links = new List<Link>();

            try
            {
                // Generate self link
                string productLink = _linkGenerator.GetUriByAction(
                    action: "GetProductById",
                    controller: "Product",
                    values: new { productId },
                    httpContext: httpContext
                );

                if (!string.IsNullOrEmpty(productLink))
                {
                    links.Add(new Link(
                        href: productLink,
                        rel: "self",
                        method: "GET"
                    ));
                }
                else
                {
                    Console.WriteLine($"Failed to generate self link for product ID {productId}");
                }

                // Generate "all-products" link with an empty object to avoid unnecessary parameters
                string allProductsLink = _linkGenerator.GetUriByAction(
                    action: "GetAllProducts",
                    controller: "Product",
                    values: new { }, // Explicitly empty to prevent unwanted parameters
                    httpContext: httpContext
                );

                if (!string.IsNullOrEmpty(allProductsLink))
                {
                    links.Add(new Link(
                        href: allProductsLink,
                        rel: "all-products",
                        method: "GET"
                    ));
                }
                else
                {
                    Console.WriteLine("Failed to generate all-products link");
                }

                // Admin & Advanced User Actions
                if (userRoles.Contains("Admin") || userRoles.Contains("Advanced User"))
                {
                    string deleteLink = _linkGenerator.GetUriByAction(
                        action: "DeleteProduct",
                        controller: "Product",
                        values: new { productId },
                        httpContext: httpContext
                    );

                    string updateLink = _linkGenerator.GetUriByAction(
                        action: "UpdateProduct",
                        controller: "Product",
                        values: new { productId },
                        httpContext: httpContext
                    );

                    if (!string.IsNullOrEmpty(deleteLink))
                    {
                        links.Add(new Link(
                            href: deleteLink,
                            rel: "delete",
                            method: "DELETE"
                        ));
                    }
                    else
                    {
                        Console.WriteLine($"Failed to generate DELETE link for product ID {productId}");
                    }

                    if (!string.IsNullOrEmpty(updateLink))
                    {
                        links.Add(new Link(
                            href: updateLink,
                            rel: "update",
                            method: "PUT"
                        ));
                    }
                    else
                    {
                        Console.WriteLine($"Failed to generate PUT link for product ID {productId}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating links for product ID {productId}: {ex.Message}");
            }

            return links;
        }
    }
}
