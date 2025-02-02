using AutoMapper;
using HotChocolate.Subscriptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Npgsql.Internal;
using SolrNet;
using System.Drawing;
using System.Reflection;
using WebStoreApp.Data;
using WebStoreApp.DTOs;
using WebStoreApp.Exceptions;
using WebStoreApp.Models;
using WebStoreApp.Repositories.Implementations;
using WebStoreApp.Repositories.Interfaces;
using WebStoreApp.Services.Interfaces;

namespace WebStoreApp.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;
        private readonly ILogger<ProductService> _logger;
        private readonly IMemoryCache _cache;
        private readonly ITopicEventSender _eventSender;
        private readonly ISolrOperations<Product> _solr;

        public ProductService(IProductRepository repository, IMapper mapper, IOrderRepository orderRepository, AppDbContext context, ILogger<ProductService> logger, IMemoryCache cache, ITopicEventSender eventSender, ISolrOperations<Product> solr)
        {
            _repository = repository;
            _mapper = mapper;
            _orderRepository = orderRepository;
            _context = context;
            _logger = logger;
            _cache = cache;
            _eventSender = eventSender;
            _solr = solr;

        }
        public async Task AddProduct(ProductDTO productDto)
        {
            try
            {
                if (productDto == null)
                {
                    _logger.LogWarning("Product data is null.");
                    throw new ServiceException("Product data cannot be null.");
                }

                var product = _mapper.Map<Product>(productDto);

                var existingCategory = await _context.Categories
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Name == productDto.CategoryName);
                if (existingCategory != null)
                {
                    product.CategoryId = existingCategory.Id;
                    product.Category = null;
                }
                else
                {
                    var newCategory = new Category { Name = productDto.CategoryName };
                    _context.Categories.Add(newCategory);
                    await _context.SaveChangesAsync();
                    product.CategoryId = newCategory.Id;
                    product.Category = null;
                }

                var existingBrand = await _context.Brands
                    .AsNoTracking()
                    .FirstOrDefaultAsync(b => b.Name == productDto.BrandName);
                if (existingBrand != null)
                {
                    product.BrandId = existingBrand.Id;
                    product.Brand = null;
                }
                else
                {
                    var newBrand = new Brand { Name = productDto.BrandName };
                    _context.Brands.Add(newBrand);
                    await _context.SaveChangesAsync();
                    product.BrandId = newBrand.Id;
                    product.Brand = null;
                }

                var existingGender = await _context.Genders
                    .AsNoTracking()
                    .FirstOrDefaultAsync(g => g.Name == productDto.GenderName);
                if (existingGender != null)
                {
                    product.GenderId = existingGender.Id;
                    product.Gender = null;
                }
                else
                {
                    var newGender = new Gender { Name = productDto.GenderName };
                    _context.Genders.Add(newGender);
                    await _context.SaveChangesAsync();
                    product.GenderId = newGender.Id;
                    product.Gender = null;
                }

                var existingColor = await _context.Colors
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Name == productDto.ColorName);
                if (existingColor != null)
                {
                    product.ColorId = existingColor.Id;
                    product.Color = null;
                }
                else
                {
                    var newColor = new Models.Color { Name = productDto.ColorName };
                    _context.Colors.Add(newColor);
                    await _context.SaveChangesAsync();
                    product.ColorId = newColor.Id;
                    product.Color = null;
                }

                var existingSize = await _context.Sizes
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s => s.Name == productDto.SizeName);
                if (existingSize != null)
                {
                    product.SizeId = existingSize.Id;
                    product.Size = null;
                }
                else
                {
                    var newSize = new Models.Size { Name = productDto.SizeName };
                    _context.Sizes.Add(newSize);
                    await _context.SaveChangesAsync();
                    product.SizeId = newSize.Id;
                    product.Size = null;
                }

                await _repository.AddProduct(product);

                _cache.Remove("all_products");
                _logger.LogInformation("Cache invalidated after adding a product.");

                try
                {
                    _solr.Add(product);
                    _solr.Commit();
                    _logger.LogInformation("Product indexed successfully in Solr.");
                }
                catch (Exception solrEx)
                {
                    _logger.LogError(solrEx, "Solr indexing failed for product: {ProductId}", product.Id);
                }
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, "Error occurred while adding a product.");
                throw new ServiceException("An error occurred while adding the product.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while adding a product.");
                throw new ServiceException("An unexpected error occurred while adding the product.", ex);
            }
        }




        public async Task<IEnumerable<ProductDTO>> AdvancedProductSearch(string? category, string? gender, string? brand, string? size, string? color, bool? inStock)
        {
            try
            {
                var products = await _repository.GetAllProducts();

                var filteredProducts = products
                    .Where(p => string.IsNullOrEmpty(category) || p.Category.Name.Equals(category, StringComparison.OrdinalIgnoreCase))
                    .Where(p => string.IsNullOrEmpty(gender) || p.Gender.Name.Equals(gender, StringComparison.OrdinalIgnoreCase))
                    .Where(p => string.IsNullOrEmpty(brand) || p.Brand.Name.Equals(brand, StringComparison.OrdinalIgnoreCase))
                    .Where(p => string.IsNullOrEmpty(size) || p.Size.Name.Equals(size, StringComparison.OrdinalIgnoreCase))
                    .Where(p => string.IsNullOrEmpty(color) || p.Color.Name.Equals(color, StringComparison.OrdinalIgnoreCase))
                    .Where(p => !inStock.HasValue || (inStock.Value ? p.Quantity > 0 : p.Quantity <= 0));

                return _mapper.Map<IEnumerable<ProductDTO>>(filteredProducts);
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving products for advanced search.");
                throw new ServiceException("An error occurred while retrieving products for advanced search.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred during advanced product search.");
                throw new ServiceException("An unexpected error occurred during product search.", ex);
            }
        }

        public async Task DeleteProduct(int ProductId)
        {
            try
            {
                if (ProductId <= 0)
                {
                    _logger.LogWarning("Invalid Product ID: {ProductId}", ProductId);
                    throw new ServiceException("Invalid Product ID.");
                }

                await _repository.DeleteProduct(ProductId);

                _cache.Remove("all_products");
                _logger.LogInformation("Cache invalidated after deleting a product.");
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, "Error occurred while deleting product with ID: {ProductId}", ProductId);
                throw new ServiceException("An error occurred while deleting the product.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while deleting product with ID: {ProductId}", ProductId);
                throw new ServiceException("An unexpected error occurred while deleting the product.", ex);
            }
        }

        public async Task<IEnumerable<ProductDTO>> GetAllProducts()
        {
            string cacheKey = "all_products";

            try
            {
                if (_cache.TryGetValue(cacheKey, out IEnumerable<ProductDTO>? cachedProducts))
                {
                    _logger.LogInformation("Returning products from cache.");
                    return cachedProducts!;
                }

                var products = await _repository.GetAllProducts();
                var productDTOs = _mapper.Map<IEnumerable<ProductDTO>>(products);

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));

                _cache.Set(cacheKey, productDTOs, cacheOptions);
                _logger.LogInformation("Products fetched from repository and cached.");

                return productDTOs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while retrieving all products.");
                throw new ServiceException("An unexpected error occurred while retrieving all products.", ex);
            }
        }

        public async Task<IEnumerable<ProductDTO>> GetOutOfStockProducts()
        {
            try
            {
                var products = await _repository.GetOutOfStockProducts();
                return _mapper.Map<IEnumerable<ProductDTO>>(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while retrieving out-of-stock products.");
                throw new ServiceException("An unexpected error occurred while retrieving out-of-stock products.", ex);
            }
        }

        public async Task<ProductDTO> GetProductById(int Id)
        {
            try
            {
                if (Id <= 0)
                {
                    _logger.LogWarning("Invalid Product ID: {Id}", Id);
                    throw new ServiceException("Invalid Product ID.");
                }

                var product = await _repository.GetProductById(Id);
                if (product == null)
                {
                    _logger.LogWarning("Product with ID {Id} not found.", Id);
                    throw new ServiceException($"Product with ID {Id} not found.");
                }

                return _mapper.Map<ProductDTO>(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while retrieving product by ID: {Id}", Id);
                throw new ServiceException("An unexpected error occurred while retrieving the product.", ex);
            }
        }

        public async Task<IEnumerable<ProductDTO>> GetProductsByBrand(string Brand)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Brand))
                {
                    _logger.LogWarning("Brand name is null or empty.");
                    throw new ServiceException("Brand name cannot be null or empty.");
                }

                var products = await _repository.GetProductsByBrand(Brand);
                return _mapper.Map<IEnumerable<ProductDTO>>(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while retrieving products by brand: {Brand}", Brand);
                throw new ServiceException("An unexpected error occurred while retrieving products by brand.", ex);
            }
        }

        public async Task<IEnumerable<ProductDTO>> GetProductsByCategory(string Category)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Category))
                {
                    _logger.LogWarning("Category name is null or empty.");
                    throw new ServiceException("Category name cannot be null or empty.");
                }

                var products = await _repository.GetProductsByCategory(Category);
                return _mapper.Map<IEnumerable<ProductDTO>>(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while retrieving products by category: {Category}", Category);
                throw new ServiceException("An unexpected error occurred while retrieving products by category.", ex);
            }
        }

        public async Task<IEnumerable<ProductDTO>> GetProductsByColor(string Color)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Color))
                {
                    _logger.LogWarning("Color name is null or empty.");
                    throw new ServiceException("Color name cannot be null or empty.");
                }

                var products = await _repository.GetProductsByColor(Color);
                return _mapper.Map<IEnumerable<ProductDTO>>(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while retrieving products by color: {Color}", Color);
                throw new ServiceException("An unexpected error occurred while retrieving products by color.", ex);
            }
        }

        public async Task<IEnumerable<ProductDTO>> GetProductsByGender(string Gender)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Gender))
                {
                    _logger.LogWarning("Gender name is null or empty.");
                    throw new ServiceException("Gender name cannot be null or empty.");
                }

                var products = await _repository.GetProductsByGender(Gender);
                return _mapper.Map<IEnumerable<ProductDTO>>(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while retrieving products by gender: {Gender}", Gender);
                throw new ServiceException("An unexpected error occurred while retrieving products by gender.", ex);
            }
        }

        public async Task<IEnumerable<ProductDTO>> GetProductsBySize(string Size)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Size))
                {
                    _logger.LogWarning("Size name is null or empty.");
                    throw new ServiceException("Size name cannot be null or empty.");
                }

                var products = await _repository.GetProductsBySize(Size);
                return _mapper.Map<IEnumerable<ProductDTO>>(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while retrieving products by size: {Size}", Size);
                throw new ServiceException("An unexpected error occurred while retrieving products by size.", ex);
            }
        }

        public async Task<IEnumerable<ProductDTO>> GetProductsWithDiscount()
        {
            try
            {
                var products = await _repository.GetProductsWithDiscount();
                return _mapper.Map<IEnumerable<ProductDTO>>(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while retrieving products with discount.");
                throw new ServiceException("An unexpected error occurred while retrieving products with discount.", ex);
            }
        }

        public async Task UpdateProduct(int productId, ProductDTO productDto)
        {
            try
            {
                if (productDto == null)
                {
                    _logger.LogWarning("Product data is null.");
                    throw new ServiceException("Product data cannot be null.");
                }

                var product = await _repository.GetProductById(productId);
                if (product == null)
                {
                    _logger.LogWarning("Product with ID {ProductId} not found.", productId);
                    throw new ServiceException($"Product with ID {productId} not found.");
                }

                product.Name = productDto.Name;
                product.Description = productDto.Description;
                product.Price = productDto.Price;
                product.Quantity = productDto.Quantity;

                var existingCategory = await _context.Categories
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Name == productDto.CategoryName);
                if (existingCategory != null)
                {
                    product.CategoryId = existingCategory.Id;
                    product.Category = null;
                }
                else
                {
                    var newCategory = new Category { Name = productDto.CategoryName };
                    await _context.Categories.AddAsync(newCategory);
                    await _context.SaveChangesAsync();
                    product.CategoryId = newCategory.Id;
                }

                var existingBrand = await _context.Brands
                    .AsNoTracking()
                    .FirstOrDefaultAsync(b => b.Name == productDto.BrandName);
                if (existingBrand != null)
                {
                    product.BrandId = existingBrand.Id;
                    product.Brand = null;
                }
                else
                {
                    var newBrand = new Brand { Name = productDto.BrandName };
                    await _context.Brands.AddAsync(newBrand);
                    await _context.SaveChangesAsync();
                    product.BrandId = newBrand.Id;
                }

                var existingGender = await _context.Genders
                    .AsNoTracking()
                    .FirstOrDefaultAsync(g => g.Name == productDto.GenderName);
                if (existingGender != null)
                {
                    product.GenderId = existingGender.Id;
                    product.Gender = null;
                }
                else
                {
                    var newGender = new Gender { Name = productDto.GenderName };
                    await _context.Genders.AddAsync(newGender);
                    await _context.SaveChangesAsync();
                    product.GenderId = newGender.Id;
                }

                var existingColor = await _context.Colors
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Name == productDto.ColorName);
                if (existingColor != null)
                {
                    product.ColorId = existingColor.Id;
                    product.Color = null;
                }
                else
                {
                    var newColor = new Models.Color { Name = productDto.ColorName };
                    await _context.Colors.AddAsync(newColor);
                    await _context.SaveChangesAsync();
                    product.ColorId = newColor.Id;
                }

                var existingSize = await _context.Sizes
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s => s.Name == productDto.SizeName);
                if (existingSize != null)
                {
                    product.SizeId = existingSize.Id;
                    product.Size = null;
                }
                else
                {
                    var newSize = new Models.Size { Name = productDto.SizeName };
                    await _context.Sizes.AddAsync(newSize);
                    await _context.SaveChangesAsync();
                    product.SizeId = newSize.Id;
                }

                await _repository.UpdateProduct(productId, product);

                _cache.Remove("all_products");
                _logger.LogInformation("Cache invalidated after updateing a product.");
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, "Error occurred while updating product with ID {ProductId}.", productId);
                throw new ServiceException("An error occurred while updating the product.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while updating product with ID {ProductId}.", productId);
                throw new ServiceException("An unexpected error occurred while updating the product.", ex);
            }
        }

        public async Task<ProductQuantityDTO> GetRealTimeProductQuantity(int productId)
        {
            try
            {
                if (productId <= 0)
                {
                    _logger.LogWarning("Invalid Product ID: {ProductId}", productId);
                    throw new ServiceException("Invalid Product ID.");
                }

                var product = await _repository.GetProductById(productId);
                if (product == null)
                {
                    _logger.LogWarning("Product with ID {ProductId} not found.", productId);
                    throw new ServiceException($"Product with ID {productId} not found.");
                }

                var soldQuantity = await _orderRepository.GetTotalQuantitySold(productId);
                var currentQuantity = product.Quantity - soldQuantity;

                return new ProductQuantityDTO
                {
                    ProductId = product.Id,
                    Name = product.Name,
                    InitialQuantity = product.Quantity,
                    SoldQuantity = soldQuantity,
                    CurrentQuantity = currentQuantity
                };
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving real-time quantity for product ID {ProductId}.", productId);
                throw new ServiceException("An error occurred while retrieving the product quantity.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while retrieving real-time quantity for product ID {ProductId}.", productId);
                throw new ServiceException("An unexpected error occurred while retrieving the product quantity.", ex);
            }
        }

        public async Task<IEnumerable<ProductDTO>> FindByPriceRange(double minPrice, double maxPrice)
        {
            try
            {
                if (minPrice < 0 || maxPrice < 0 || minPrice > maxPrice)
                {
                    _logger.LogWarning("Invalid price range: MinPrice = {MinPrice}, MaxPrice = {MaxPrice}", minPrice, maxPrice);
                    throw new ServiceException("Invalid price range. Ensure that minPrice <= maxPrice and both are non-negative.");
                }

                var products = await _repository.FindByPriceRange(minPrice, maxPrice);
                return products?.Select(product => _mapper.Map<ProductDTO>(product)) ?? Enumerable.Empty<ProductDTO>();
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving products in price range: MinPrice = {MinPrice}, MaxPrice = {MaxPrice}", minPrice, maxPrice);
                throw new ServiceException("An error occurred while retrieving products in the specified price range.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while retrieving products in price range: MinPrice = {MinPrice}, MaxPrice = {MaxPrice}", minPrice, maxPrice);
                throw new ServiceException("An unexpected error occurred while retrieving products in the specified price range.", ex);
            }
        }

        public async Task UpdateProductStock(int productId, int newStock)
        {
            var product = await _repository.GetProductById(productId);
            if (product == null)
            {
                throw new ServiceException("Product not found.");
            }

            product.Quantity = newStock;
            await _repository.UpdateProduct(productId, product);

            var productDto = _mapper.Map<ProductDTO>(product);

            await _eventSender.SendAsync("ProductStockUpdated", productDto);
            _logger.LogInformation($"Published stock update event for Product ID {productId}");
        }

        public async Task<IEnumerable<ProductDTO>> SearchProducts(string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                {
                    throw new ServiceException("Search query cannot be empty.");
                }

                var results = await _solr.QueryAsync(new SolrQuery(query));

                return _mapper.Map<IEnumerable<ProductDTO>>(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching for products in Solr.");
                throw new ServiceException("An error occurred while searching for products.", ex);
            }

        }
    }
}
