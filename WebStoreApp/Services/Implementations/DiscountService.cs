using AutoMapper;
using WebStoreApp.DTOs;
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

        public DiscountService(IProductRepository productRepository, IDiscountRepository discountRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _discountRepository = discountRepository;
            _mapper = mapper;
        }

        public async Task AddDiscount(DiscountDTO discountDto)
        {
            var discount = _mapper.Map<Discount>(discountDto);
            await _discountRepository.AddDiscount(discount);
        }

        public async Task<string> ApplyDiscountToBrand(string brandName, int discountId)
        {
            var products = await _productRepository.GetProductsByBrand(brandName);
            var discount = await _discountRepository.GetDiscountById(discountId);

            if (discount == null)
                return "Discount not found";

            foreach (var product in products)
            {
                product.DiscountedPrice = CalculateDiscountedPrice(product.Price, discount.DisountPercentage);
                product.Discounts.Add(discount);

                await _productRepository.UpdateProduct(product.Id, product);
            }
            return "Discount applied to brand successfully";
        }

        public async Task<string> ApplyDiscountToCategory(string categoryName, int discountId)
        {
            var products = await _productRepository.GetProductsByCategory(categoryName);
            var discount = await _discountRepository.GetDiscountById(discountId);

            if (discount == null)
                return "Discount not found";

            foreach (var product in products)
            {
                product.DiscountedPrice = CalculateDiscountedPrice(product.Price, discount.DisountPercentage);
                product.Discounts.Add(discount);

                await _productRepository.UpdateProduct(product.Id, product);
            }
            return "Discount applied to category successfully";
        }

        public async Task<string> ApplyDiscountToProduct(int productId, int discountId)
        {
            var product = await _productRepository.GetProductById(productId);
            var discount = await _discountRepository.GetDiscountById(discountId);

            if (product == null || discount == null)
                return "Product or discount not found";

            product.DiscountedPrice = CalculateDiscountedPrice(product.Price, discount.DisountPercentage);
            product.Discounts.Add(discount);

            await _productRepository.UpdateProduct(product.Id, product);
            return "Discount applied to product successfully";
        }

        private double CalculateDiscountedPrice(double price, double discountPercentage)
        {
            return price - (price * (discountPercentage / 100.0));
        }


        public async Task DeleteDiscount(int discountId)
        {
            await _discountRepository.DeleteDiscount(discountId);
        }

        public async Task<IEnumerable<DiscountDTO>> GetAllDiscounts()
        {
            var discounts = await _discountRepository.GetAllDiscounts();
            return _mapper.Map<IEnumerable<DiscountDTO>>(discounts);
        }

        public async Task<DiscountDTO> GetDiscountById(int DiscountId)
        {
            var discount = await _discountRepository.GetDiscountById(DiscountId);
            return _mapper.Map<DiscountDTO>(discount);
        }

        public async Task<IEnumerable<DiscountDTO>> GetDiscountsByDateRange(DateTime startDate, DateTime endDate)
        {
            var discounts = await _discountRepository.GetDiscountsByDateRange(startDate, endDate);
            return _mapper.Map<IEnumerable<DiscountDTO>>(discounts);
        }

        public async Task<IEnumerable<DiscountDTO>> GetDiscountsByEndingDate(DateTime endDate)
        {
            var discounts = await _discountRepository.GetDiscountsByEndingDate(endDate);
            return _mapper.Map<IEnumerable<DiscountDTO>>(discounts);
        }

        public async Task<IEnumerable<DiscountDTO>> GetDiscountsByName(string name)
        {
            var discounts = await _discountRepository.GetDiscountsByName(name);
            return _mapper.Map<IEnumerable<DiscountDTO>>(discounts);
        }

        public async Task<IEnumerable<DiscountDTO>> GetDiscountsByStartingDate(DateTime startDate)
        {
            var discounts = await _discountRepository.GetDiscountsByStartingDate(startDate);
            return _mapper.Map<IEnumerable<DiscountDTO>>(discounts);
        }

        public async Task RemoveExpiredDiscounts()
        {
            var allProducts = await _productRepository.GetAllProducts();
            var dateTimeNow = DateTime.UtcNow;

            foreach (var product in allProducts)
            {
                var activeDiscounts = product.Discounts.Where(d => d.EndDate > dateTimeNow).ToList();
                product.Discounts = activeDiscounts;

                if (!activeDiscounts.Any())
                    product.DiscountedPrice = 0;

                await _productRepository.UpdateProduct(product.Id, product);
            }
        }

        public async Task UpdateDiscount(int discountId, DiscountDTO discountDto)
        {
            var discount = _mapper.Map<Discount>(discountDto);
            await _discountRepository.UpdateDiscount(discountId, discount);
        }

    }
}
