using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using WebStoreApp.Models;

namespace WebStoreApp.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Models.Color> Colors { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<Gender> Genders { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Models.Size> Sizes { get; set; }
    }
}
