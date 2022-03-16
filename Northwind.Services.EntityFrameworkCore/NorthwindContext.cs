using Microsoft.EntityFrameworkCore;
using Northwind.DataAccess.Employees;
using Northwind.DataAccess.Products;
using Northwind.Services.Employees;
using Northwind.Services.Products;

namespace Northwind.Services.EntityFrameworkCore
{
    /// <summary>
    /// Context required by entity framework.
    /// </summary>
    public class NorthwindContext : DbContext
    {
        private readonly string connectionString;

        /// <summary>
        /// Initialize the new instance of class <see cref="NorthwindContext"/>
        /// </summary>
        /// <param name="connectionString"></param>
        public NorthwindContext(string connectionString)
        {
            this.connectionString = connectionString;
        }

        /// <summary>
        /// Employees data context.
        /// </summary>
        public virtual DbSet<EmployeeTransferObject> Employees { get; set; }
        
        /// <summary>
        /// Products data context.
        /// </summary>
        public virtual DbSet<ProductTransferObject> Products { get; set; }

        /// <summary>
        /// Categories data context.
        /// </summary>
        public virtual DbSet<ProductCategoryTransferObject> Categories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(this.connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductTransferObject>()
                .Property(b => b.Id)
                .HasColumnName("ProductID");

            modelBuilder.Entity<ProductTransferObject>()
                .Property(b => b.Name)
                .HasColumnName("ProductName");

            modelBuilder.Entity<ProductCategoryTransferObject>()
                .Property(b => b.Id)
                .HasColumnName("CategoryID");

            modelBuilder.Entity<ProductCategoryTransferObject>()
                .Property(b => b.Name)
                .HasColumnName("CategoryName");

            modelBuilder.Entity<EmployeeTransferObject>()
                .Property(b => b.Id)
                .HasColumnName("EmployeeID");
        }
    }
}