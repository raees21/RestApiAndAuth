using Microsoft.EntityFrameworkCore;
using RestAPI.Models;

namespace RestAPI.DataAccess;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options) {}
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductVariant> ProductVariants { get; set; }
    public DbSet<ProductType> ProductTypes { get; set; }
    public DbSet<ShoeSize> ShoeSizes { get; set; }
    public DbSet<FootSide> FootSides { get; set; }

    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<UserType> UserTypes { get; set; }
    public DbSet<Gender> Genders { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderType> OrderTypes { get; set; }
    public DbSet<OrderStatus> OrderStatuses { get; set; }
    public DbSet<OrderStatusUpdate> OrderStatusUpdates { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OrderItem>().HasKey(orderItem => new { orderItem.OrderId, orderItem.ProductVariantId });
        
        modelBuilder.Entity<UserType>()
            .Property(userType => userType.Type)
            .HasConversion<string>();
        
        modelBuilder.Entity<Gender>()
            .Property(gender => gender.Name)
            .HasConversion<string>();
        
        modelBuilder.Entity<OrderType>()
            .Property(orderType => orderType.Type)
            .HasConversion<string>();

        modelBuilder.Entity<ProductType>()
            .Property(productType => productType.Type)
            .HasConversion<string>();

        modelBuilder.Entity<OrderStatus>()
            .Property(orderStatus => orderStatus.Status)
            .HasConversion<string>();
        
        modelBuilder.Entity<ShoeSize>()
            .Property(shoeSize => shoeSize.Code)
            .HasConversion<string>();
        
        modelBuilder.Entity<FootSide>()
            .Property(footSide => footSide.Side)
            .HasConversion<string>();
        
        modelBuilder.Entity<UserProfile>().ToTable("UserProfile");
        modelBuilder.Entity<UserType>().ToTable("UserType");
        modelBuilder.Entity<Gender>().ToTable("Gender");
        modelBuilder.Entity<Address>().ToTable("Address");
        modelBuilder.Entity<Order>().ToTable("Order");
        modelBuilder.Entity<OrderStatus>().ToTable("OrderStatus");
        modelBuilder.Entity<OrderStatusUpdate>().ToTable("OrderStatusUpdate");
        modelBuilder.Entity<OrderType>().ToTable("OrderType");
        modelBuilder.Entity<Product>().ToTable("Product");
        modelBuilder.Entity<ProductType>().ToTable("ProductType");
        modelBuilder.Entity<ProductVariant>().ToTable("ProductVariant");
        modelBuilder.Entity<ShoeSize>().ToTable("ShoeSize");
        modelBuilder.Entity<FootSide>().ToTable("FootSide");
    }
}