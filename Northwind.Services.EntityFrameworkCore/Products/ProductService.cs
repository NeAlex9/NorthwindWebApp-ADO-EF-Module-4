using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Northwind.Services.Products;

namespace Northwind.Services.EntityFrameworkCore.Products
{
    public class ProductService : IProductService
    {
        private readonly NorthwindContext context;

        public ProductService(NorthwindContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<Product> GetProductsAsync(int offset, int limit)
        {
            await foreach (var product in this.context
                               .Products
                               .Skip(offset)
                               .Take(limit)
                               .AsAsyncEnumerable())
            {
                yield return product;
            }
        }

        /// <inheritdoc />
        public async Task<(bool isSuccess, Product product)> TryGetProductAsync(int productId)
        {
            var product = await this.context
                .Products
                .Where(product => product.Id == productId)
                .FirstOrDefaultAsync();
            if (product is null)
            {
                return (false, null);
            }

            return (true, product);
        }

        /// <inheritdoc />
        public async Task<int> CreateProductAsync(Product product)
        {
            if (product is null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            await this.context
                .AddAsync(product);
            return product.Id;
        }

        /// <inheritdoc />
        public async Task<bool> DeleteProductAsync(int productId)
        {
            var product = await this.context
                .Products
                .Where(product => productId == product.Id)
                .FirstOrDefaultAsync();
            this.context
                .Remove(product);
            return await this.context
                .SaveChangesAsync() > 0;

        }

        /// <inheritdoc />
        public async IAsyncEnumerable<Product> GetProductsByNameAsync(ICollection<string> names)
        {
            if (names is null)
            {
                throw new ArgumentNullException(nameof(names));
            }

            var products = this.context.Products
                .Where(product => names.Any(name => name == product.Name))
                .AsAsyncEnumerable();
            await foreach (var product in products)
            {
                yield return product;
            }
        }

        /// <inheritdoc />
        public async Task<bool> UpdateProductAsync(int productId, Product product)
        {
            if (product is null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            var updatedProduct = await this.context
                .Products
                .Where(p => productId == p.Id)
                .FirstOrDefaultAsync();
            if (updatedProduct is not null)
            {
                updatedProduct.Name = product.Name;
                updatedProduct.CategoryId = product.CategoryId;
                updatedProduct.Discontinued = product.Discontinued;
                updatedProduct.QuantityPerUnit = product.QuantityPerUnit;
                updatedProduct.ReorderLevel = product.ReorderLevel;
                updatedProduct.SupplierId = product.SupplierId;
                updatedProduct.UnitPrice = product.UnitPrice;
                updatedProduct.UnitsInStock = product.UnitsInStock;
                updatedProduct.UnitsOnOrder = product.UnitsOnOrder;
            }

            return await this.context.SaveChangesAsync() > 0;
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<Product> GetProductsByCategoryAsync(ICollection<int> categoriesId)
        {
            if (categoriesId is null)
            {
                throw new ArgumentNullException(nameof(categoriesId));
            }

            var products = this.context.Products
                .Where(product => categoriesId.Any(id => id == product.Id))
                .AsAsyncEnumerable();
            await foreach (var product in products)
            {
                yield return product;
            }
        }
    }
}
