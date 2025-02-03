using AutoMapper;
using WebStoreApp.DTOs;
using WebStoreApp.Exceptions;
using WebStoreApp.Models;
using WebStoreApp.Repositories.Interfaces;
using WebStoreApp.Services.Interfaces;

namespace WebStoreApp.Services.Implementations
{
    public class DiscountService : IDiscountService
    {
        private readonly IProductRepository _productRepository;
        private readonly IDiscountRepository _discountRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<DiscountService> _logger;


        public DiscountService(IProductRepository productRepository, IDiscountRepository discountRepository, IMapper mapper, ILogger<DiscountService> logger)
        {
            _productRepository = productRepository;
            _discountRepository = discountRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task AddDiscount(DiscountDTO discountDto)
        {
            try
            {
                if (discountDto == null)
                {
                    _logger.LogWarning("AddDiscount: Discount data is null.");
                    throw new ServiceException("Discount data cannot be null.");
                }

                var discount = _mapper.Map<Discount>(discountDto);
                await _discountRepository.AddDiscount(discount);
                _logger.LogInformation("AddDiscount: Discount added successfully with name: {Name}.", discount.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AddDiscount: An error occurred while adding the discount.");
                throw new ServiceException("An unexpected error occurred while adding the discount.", ex);
            }
        }


        public async Task<string> ApplyDiscountToBrand(string brandName, int discountId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(brandName))
                {
                    _logger.LogWarning("ApplyDiscountToBrand: Brand name is null or empty.");
                    throw new ServiceException("Brand name cannot be null or empty.");
                }

                if (discountId <= 0)
                {
                    _logger.LogWarning("ApplyDiscountToBrand: Invalid discount ID: {DiscountId}.", discountId);
                    throw new ServiceException("Discount ID must be a valid positive integer.");
                }

                var products = await _productRepository.GetProductsByBrand(brandName);
                if (!products.Any())
                {
                    _logger.LogWarning("ApplyDiscountToBrand: No products found for brand: {BrandName}.", brandName);
                    throw new ServiceException($"No products found for brand: {brandName}.");
                }

                var discount = await _discountRepository.GetDiscountById(discountId);
                if (discount == null)
                {
                    _logger.LogWarning("ApplyDiscountToBrand: Discount with ID {DiscountId} not found.", discountId);
                    throw new ServiceException($"Discount with ID {discountId} not found.");
                }

                foreach (var product in products)
                {
                    product.DiscountedPrice = CalculateDiscountedPrice(product.Price, discount.DiscountPercentage);
                    product.Discounts.Add(discount);
                    await _productRepository.UpdateProduct(product.Id, product);
                }

                _logger.LogInformation("ApplyDiscountToBrand: Discount with ID {DiscountId} applied to brand {BrandName}.", discountId, brandName);
                return "Discount applied to brand successfully";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ApplyDiscountToBrand: An error occurred while applying discount with ID {DiscountId} to brand {BrandName}.", discountId, brandName);
                throw new ServiceException($"An error occurred while applying discount to brand: {brandName}.", ex);
            }
        }


        public async Task<string> ApplyDiscountToCategory(string categoryName, int discountId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(categoryName))
                {
                    _logger.LogWarning("ApplyDiscountToCategory: Category name is null or empty.");
                    throw new ServiceException("Category name cannot be null or empty.");
                }

                if (discountId <= 0)
                {
                    _logger.LogWarning("ApplyDiscountToCategory: Invalid discount ID: {DiscountId}.", discountId);
                    throw new ServiceException("Discount ID must be a valid positive integer.");
                }

                var products = await _productRepository.GetProductsByCategory(categoryName);
                if (!products.Any())
                {
                    _logger.LogWarning("ApplyDiscountToCategory: No products found for category: {CategoryName}.", categoryName);
                    throw new ServiceException($"No products found for category: {categoryName}.");
                }

                var discount = await _discountRepository.GetDiscountById(discountId);
                if (discount == null)
                {
                    _logger.LogWarning("ApplyDiscountToCategory: Discount with ID {DiscountId} not found.", discountId);
                    throw new ServiceException($"Discount with ID {discountId} not found.");
                }

                foreach (var product in products)
                {
                    product.DiscountedPrice = CalculateDiscountedPrice(product.Price, discount.DiscountPercentage);
                    product.Discounts.Add(discount);
                    await _productRepository.UpdateProduct(product.Id, product);
                }

                _logger.LogInformation("ApplyDiscountToCategory: Discount with ID {DiscountId} applied to category {CategoryName}.", discountId, categoryName);
                return "Discount applied to category successfully";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ApplyDiscountToCategory: An error occurred while applying discount with ID {DiscountId} to category {CategoryName}.", discountId, categoryName);
                throw new ServiceException($"An error occurred while applying discount to category: {categoryName}.", ex);
            }
        }


        public async Task<string> ApplyDiscountToProduct(int productId, int discountId)
        {
            try
            {
                if (productId <= 0 || discountId <= 0)
                {
                    _logger.LogWarning("ApplyDiscountToProduct: Invalid product ID {ProductId} or discount ID {DiscountId}.", productId, discountId);
                    throw new ServiceException("Product ID and Discount ID must be valid positive integers.");
                }

                var product = await _productRepository.GetProductById(productId);
                if (product == null)
                {
                    _logger.LogWarning("ApplyDiscountToProduct: Product with ID {ProductId} not found.", productId);
                    throw new ServiceException($"Product with ID {productId} not found.");
                }

                var discount = await _discountRepository.GetDiscountById(discountId);
                if (discount == null)
                {
                    _logger.LogWarning("ApplyDiscountToProduct: Discount with ID {DiscountId} not found.", discountId);
                    throw new ServiceException($"Discount with ID {discountId} not found.");
                }

                product.DiscountedPrice = CalculateDiscountedPrice(product.Price, discount.DiscountPercentage);
                product.Discounts.Add(discount);

                await _productRepository.UpdateProduct(product.Id, product);
                _logger.LogInformation("ApplyDiscountToProduct: Discount with ID {DiscountId} applied successfully to product ID {ProductId}.", discountId, productId);

                return "Discount applied to product successfully";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ApplyDiscountToProduct: An error occurred while applying discount with ID {DiscountId} to product ID {ProductId}.", discountId, productId);
                throw new ServiceException($"An error occurred while applying discount to product with ID: {productId}.", ex);
            }
        }

        private double CalculateDiscountedPrice(double price, double discountPercentage)
        {
            if (price < 0 || discountPercentage < 0)
            {
                _logger.LogWarning("CalculateDiscountedPrice: Invalid inputs. Price: {Price}, DiscountPercentage: {DiscountPercentage}.", price, discountPercentage);
                throw new ServiceException("Price and Discount Percentage must be non-negative.");
            }

            return price - (price * (discountPercentage / 100.0));
        }

        public async Task DeleteDiscount(int discountId)
        {
            try
            {
                if (discountId <= 0)
                {
                    _logger.LogWarning("DeleteDiscount: Invalid discount ID: {DiscountId}.", discountId);
                    throw new ServiceException("Discount ID must be a valid positive integer.");
                }

                await _discountRepository.DeleteDiscount(discountId);
                _logger.LogInformation("DeleteDiscount: Discount with ID {DiscountId} deleted successfully.", discountId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteDiscount: An error occurred while deleting discount with ID {DiscountId}.", discountId);
                throw new ServiceException($"An error occurred while deleting discount with ID: {discountId}.", ex);
            }
        }

        public async Task<IEnumerable<DiscountDTO>> GetAllDiscounts()
        {
            try
            {
                var discounts = await _discountRepository.GetAllDiscounts();
                _logger.LogInformation("GetAllDiscounts: Retrieved all discounts successfully.");
                return _mapper.Map<IEnumerable<DiscountDTO>>(discounts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAllDiscounts: An error occurred while retrieving all discounts.");
                throw new ServiceException("An error occurred while retrieving all discounts.", ex);
            }
        }

        public async Task<DiscountDTO> GetDiscountById(int DiscountId)
        {
            try
            {
                if (DiscountId <= 0)
                {
                    _logger.LogWarning("GetDiscountById: Invalid discount ID: {DiscountId}.", DiscountId);
                    throw new ServiceException("Discount ID must be a valid positive integer.");
                }

                var discount = await _discountRepository.GetDiscountById(DiscountId);
                if (discount == null)
                {
                    _logger.LogWarning("GetDiscountById: Discount with ID {DiscountId} not found.", DiscountId);
                    throw new ServiceException($"Discount with ID {DiscountId} not found.");
                }

                _logger.LogInformation("GetDiscountById: Retrieved discount with ID {DiscountId} successfully.", DiscountId);
                return _mapper.Map<DiscountDTO>(discount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetDiscountById: An error occurred while retrieving discount with ID {DiscountId}.", DiscountId);
                throw new ServiceException($"An error occurred while retrieving discount with ID: {DiscountId}.", ex);
            }
        }

        public async Task<IEnumerable<DiscountDTO>> GetDiscountsByDateRange(DateTime startDate, DateTime endDate)
        {
            try
            {
                if (startDate > endDate)
                {
                    _logger.LogWarning("GetDiscountsByDateRange: Invalid date range. StartDate: {StartDate}, EndDate: {EndDate}.", startDate, endDate);
                    throw new ServiceException("Start date must be earlier than or equal to end date.");
                }

                var discounts = await _discountRepository.GetDiscountsByDateRange(startDate, endDate);
                _logger.LogInformation("GetDiscountsByDateRange: Retrieved discounts for date range {StartDate} - {EndDate} successfully.", startDate, endDate);
                return _mapper.Map<IEnumerable<DiscountDTO>>(discounts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetDiscountsByDateRange: An error occurred while retrieving discounts for date range {StartDate} - {EndDate}.", startDate, endDate);
                throw new ServiceException("An error occurred while retrieving discounts for the specified date range.", ex);
            }
        }

        public async Task<IEnumerable<DiscountDTO>> GetDiscountsByEndingDate(DateTime endDate)
        {
            try
            {
                var discounts = await _discountRepository.GetDiscountsByEndingDate(endDate);
                _logger.LogInformation("GetDiscountsByEndingDate: Retrieved discounts ending on {EndDate} successfully.", endDate);
                return _mapper.Map<IEnumerable<DiscountDTO>>(discounts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetDiscountsByEndingDate: An error occurred while retrieving discounts ending on {EndDate}.", endDate);
                throw new ServiceException($"An error occurred while retrieving discounts ending on {endDate}.", ex);
            }
        }

        public async Task<IEnumerable<DiscountDTO>> GetDiscountsByName(string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    _logger.LogWarning("GetDiscountsByName: Discount name is null or empty.");
                    throw new ServiceException("Discount name cannot be null or empty.");
                }

                var discounts = await _discountRepository.GetDiscountsByName(name);
                _logger.LogInformation("GetDiscountsByName: Retrieved discounts with name containing '{Name}' successfully.", name);
                return _mapper.Map<IEnumerable<DiscountDTO>>(discounts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetDiscountsByName: An error occurred while retrieving discounts with name containing '{Name}'.", name);
                throw new ServiceException($"An error occurred while retrieving discounts with name containing '{name}'.", ex);
            }
        }

        public async Task<IEnumerable<DiscountDTO>> GetDiscountsByStartingDate(DateTime startDate)
        {
            try
            {
                var discounts = await _discountRepository.GetDiscountsByStartingDate(startDate);
                _logger.LogInformation("GetDiscountsByStartingDate: Retrieved discounts starting on {StartDate} successfully.", startDate);
                return _mapper.Map<IEnumerable<DiscountDTO>>(discounts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetDiscountsByStartingDate: An error occurred while retrieving discounts starting on {StartDate}.", startDate);
                throw new ServiceException($"An error occurred while retrieving discounts starting on {startDate}.", ex);
            }
        }


        public async Task RemoveExpiredDiscounts()
        {
            try
            {
                var allProducts = await _productRepository.GetAllProducts();
                var dateTimeNow = DateTime.UtcNow;

                if (!allProducts.Any())
                {
                    _logger.LogWarning("RemoveExpiredDiscounts: No products found in the database.");
                    throw new ServiceException("No products available to process discounts.");
                }

                foreach (var product in allProducts)
                {
                    var activeDiscounts = product.Discounts.Where(d => d.EndDate > dateTimeNow).ToList();
                    product.Discounts = activeDiscounts;

                    if (!activeDiscounts.Any())
                        product.DiscountedPrice = 0;

                    await _productRepository.UpdateProduct(product.Id, product);
                }

                _logger.LogInformation("RemoveExpiredDiscounts: Successfully removed expired discounts from products.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RemoveExpiredDiscounts: An error occurred while removing expired discounts.");
                throw new ServiceException("An error occurred while removing expired discounts.", ex);
            }
        }

        public async Task UpdateDiscount(int discountId, DiscountDTO discountDto)
        {
            try
            {
                if (discountId <= 0)
                {
                    _logger.LogWarning("UpdateDiscount: Invalid discount ID: {DiscountId}.", discountId);
                    throw new ServiceException("Discount ID must be a valid positive integer.");
                }

                if (discountDto == null)
                {
                    _logger.LogWarning("UpdateDiscount: Discount data is null for discount ID: {DiscountId}.", discountId);
                    throw new ServiceException("Discount data cannot be null.");
                }

                var discount = _mapper.Map<Discount>(discountDto);
                await _discountRepository.UpdateDiscount(discountId, discount);

                _logger.LogInformation("UpdateDiscount: Successfully updated discount with ID {DiscountId}.", discountId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateDiscount: An error occurred while updating discount with ID {DiscountId}.", discountId);
                throw new ServiceException($"An error occurred while updating discount with ID: {discountId}.", ex);
            }
        }
    }
}
