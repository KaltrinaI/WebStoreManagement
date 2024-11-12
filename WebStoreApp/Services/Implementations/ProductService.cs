using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Npgsql.Internal;
using System.Drawing;
using System.Reflection;
using WebStoreApp.Data;
using WebStoreApp.DTOs;
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


        public ProductService(IProductRepository repository, IMapper mapper, IOrderRepository orderRepository, AppDbContext context)
        {
            _repository = repository;
            _mapper = mapper;
            _orderRepository = orderRepository;
            _context = context;
        }
        public async Task AddProduct(ProductDTO productDto)
        {
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
        }



        public async Task<IEnumerable<ProductDTO>> AdvancedProductSearch(string? category, string? gender, string? brand, double? minPrice, double? maxPrice, string? size, string? color, bool? inStock)
        {
            var products = await _repository.GetAllProducts();

            var filteredProducts = products
                .Where(p => string.IsNullOrEmpty(category) || p.Category.Name.Equals(category, StringComparison.OrdinalIgnoreCase))
                .Where(p => string.IsNullOrEmpty(gender) || p.Gender.Name.Equals(gender, StringComparison.OrdinalIgnoreCase))
                .Where(p => string.IsNullOrEmpty(brand) || p.Brand.Name.Equals(brand, StringComparison.OrdinalIgnoreCase))
                .Where(p => !minPrice.HasValue || p.Price >= minPrice.Value)
                .Where(p => !maxPrice.HasValue || p.Price <= maxPrice.Value)
                .Where(p => string.IsNullOrEmpty(size) || p.Size.Name.Equals(size, StringComparison.OrdinalIgnoreCase))
                .Where(p => string.IsNullOrEmpty(color) || p.Color.Name.Equals(color, StringComparison.OrdinalIgnoreCase))
                .Where(p => !inStock.HasValue || (inStock.Value ? p.Quantity > 0 : p.Quantity <= 0));

            return _mapper.Map<IEnumerable<ProductDTO>>(filteredProducts);
        }

        public async Task DeleteProduct(int ProductId)
        {
            await _repository.DeleteProduct(ProductId);
        }

        public async Task<IEnumerable<ProductDTO>> GetAllProducts()
        {
            var products = await _repository.GetAllProducts();
            return _mapper.Map<IEnumerable<ProductDTO>>(products);
        }

        public async Task<IEnumerable<ProductDTO>> GetOutOfStockProducts()
        {
            var products = await _repository.GetOutOfStockProducts();
            return _mapper.Map<IEnumerable<ProductDTO>>(products);
        }

        public async Task<ProductDTO> GetProductById(int Id)
        {
            var product = await _repository.GetProductById(Id);
            return _mapper.Map<ProductDTO>(product);
        }

        public async Task<IEnumerable<ProductDTO>> GetProductsByBrand(string Brand)
        {
            var products = await _repository.GetProductsByBrand(Brand);
            return _mapper.Map<IEnumerable<ProductDTO>>(products);
        }

        public async Task<IEnumerable<ProductDTO>> GetProductsByCategory(string Category)
        {
            var products = await _repository.GetProductsByCategory(Category);
            return _mapper.Map<IEnumerable<ProductDTO>>(products);
        }

        public async Task<IEnumerable<ProductDTO>> GetProductsByColor(string Color)
        {
            var products = await _repository.GetProductsByColor(Color);
            return _mapper.Map<IEnumerable<ProductDTO>>(products);
        }

        public async Task<IEnumerable<ProductDTO>> GetProductsByGender(string Gender)
        {
            var products = await _repository.GetProductsByGender(Gender);
            return _mapper.Map<IEnumerable<ProductDTO>>(products);
        }

        public async Task<IEnumerable<ProductDTO>> GetProductsBySize(string Size)
        {
            var products = await _repository.GetProductsBySize(Size);
            return _mapper.Map<IEnumerable<ProductDTO>>(products);
        }

        public async Task<IEnumerable<ProductDTO>> GetProductsWithDiscount()
        {
            var products = await _repository.GetProductsWithDiscount();
            return _mapper.Map<IEnumerable<ProductDTO>>(products);
        }

        public async Task UpdateProduct(int productId, ProductDTO productDto)
        {
            var product = await _repository.GetProductById(productId);
            if (product == null)
            {
                throw new InvalidOperationException("Product not found");
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
        }

        public async Task<ProductQuantityDTO> GetRealTimeProductQuantity(int productId)
        {
            var product = await _repository.GetProductById(productId);
            if (product == null)
            {
                throw new InvalidOperationException("Product not found");
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


    }
}
