using AutoMapper;
using WebStoreApp.DTOs;
using WebStoreApp.Models;

namespace WebStoreApp.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductDTO>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
            .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Brand.Name))
            .ForMember(dest => dest.GenderName, opt => opt.MapFrom(src => src.Gender.Name))
            .ForMember(dest => dest.ColorName, opt => opt.MapFrom(src => src.Color.Name))
            .ForMember(dest => dest.SizeName, opt => opt.MapFrom(src => src.Size.Name))
            .ReverseMap();

            CreateMap<Discount, DiscountDTO>();

            CreateMap<Order, OrderDTO>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems));
            CreateMap<OrderDTO, Order>();

            CreateMap<User, UserDTO>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ReverseMap();

            CreateMap<OrderItem, OrderItemDTO>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Product.Id))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));
            CreateMap<OrderItemDTO, OrderItem>();

            CreateMap<DiscountDTO, Discount>();
            CreateMap<Discount, DiscountDTO>();

            CreateMap<OrderRequestDTO, Order>()
                 .ForPath(dest => dest.User.Email, opt => opt.MapFrom(src => src.Email))
                 .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems))
                 .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.OrderDate));

            CreateMap<Product, ProductResponseDTO>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Brand.Name))
                .ForMember(dest => dest.GenderName, opt => opt.MapFrom(src => src.Gender.Name))
                .ForMember(dest => dest.ColorName, opt => opt.MapFrom(src => src.Color.Name))
                .ForMember(dest => dest.SizeName, opt => opt.MapFrom(src => src.Size.Name));

            CreateMap<OrderItem, OrderItemsResponseDTO>()
                .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.Product))
                .ForMember(dest => dest.quantity, opt => opt.MapFrom(src => src.Quantity));


            CreateMap<Order, OrderResponseDTO>()
                .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => src.OrderStatus.ToString()))
                .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.OrderDate))
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems));

            CreateMap<Report, ReportDTO>()
               .ForMember(dest => dest.ReportDate, opt => opt.MapFrom(src => src.ReportDate))
               .ForMember(dest => dest.TotalEarnings, opt => opt.MapFrom(src => src.TotalEarnings))
               .ForMember(dest => dest.Month, opt => opt.MapFrom(src => src.Month ?? 0))
               .ForMember(dest => dest.Year, opt => opt.MapFrom(src => src.Year ?? 0))
               .ForMember(dest => dest.TopSellingProduct, opt => opt.MapFrom(src => src.TopSellingProduct));

            CreateMap<ProductPerformanceReport, ProductPerformanceDTO>()
               .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductName))
               .ForMember(dest => dest.TotalSales, opt => opt.MapFrom(src => src.TotalSales))
               .ForMember(dest => dest.UnitsSold, opt => opt.MapFrom(src => src.UnitsSold));
        }
    }

}


