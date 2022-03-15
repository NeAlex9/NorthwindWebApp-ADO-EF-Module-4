using Microsoft.EntityFrameworkCore;
using Northwind.Services.Employees;
using Northwind.Services.Products;


namespace Northwind.Services.EntityFrameworkCore
{
    public class NorthwindContext : DbContext
    {
        private readonly string connectionString;

        public NorthwindContext(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductCategory> ProductCategories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(this.connectionString);
        }
    }
}