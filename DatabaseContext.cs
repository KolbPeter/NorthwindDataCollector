#pragma warning disable CS8618
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using NorthwindDataCollector.Dtos;

namespace NorthwindDataCollector
{
    public class DatabaseContext : DbContext
    {
        private readonly string connectionString;

        public DbSet<Category> Categories { get; set; }
        public DbSet<CustomerCustomerDemo> CustomerCustomerDemo { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerDemographic> CustomerDemographics { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeeTerritory> EmployeeTerritories { get; set; }
        public DbSet<OrderDetail> Order_Details { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<Shipper> Shippers { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Territory> Territories { get; set; }

        public DatabaseContext(string? connectionString = null)
        {
            this.connectionString =
                connectionString
                ?? $@"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=Northwind;Integrated Security=True;AttachDBFilename={Path.Combine(Environment.CurrentDirectory, "northwnd.mdf")}";
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (!options.IsConfigured)
            {
                options.UseSqlServer(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            builder.Entity<CustomerDemographic>().HasKey(x => x.CustomerTypeId);
            builder.Entity<CustomerCustomerDemo>().HasKey(x => new { x.CustomerTypeId, x.CustomerId });
            builder.Entity<EmployeeTerritory>().HasKey(x => new { x.EmployeeId, x.TerritoryId });
            builder.Entity<OrderDetail>().HasKey(x => new { x.OrderId, x.ProductId });
        }
    }
}