using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Northwind.DataAccess.Products;
using Northwind.Services.Products;

namespace Northwind.Services.EntityFrameworkCore.Products
{
    public class ProductService : IProductService
    {
        private readonly NorthwindContext context;
        private readonly IMapper mapper;

        public ProductService(NorthwindContext context, IMapper mapper)
        {
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<Product> GetProductsAsync(int offset, int limit)
        {
            await foreach (var productDto in this.context
                               .Products
                               .AsNoTracking()
                               .Skip(offset)
                               .Take(limit)
                               .AsAsyncEnumerable())
            {
                yield return this.mapper.Map<Product>(productDto);
            }
        }

        /// <inheritdoc />
        public async Task<(bool isSuccess, Product product)> TryGetProductAsync(int productId)
        {
            var productDto = await this.context
                .Products
                .AsNoTracking()
                .Where(product => product.Id == productId)
                .FirstOrDefaultAsync();
            if (productDto is null)
            {
                return (false, null);
            }

            return (true, this.mapper.Map<Product>(productDto));
        }

        /// <inheritdoc />
        public async Task<int> CreateProductAsync(Product product)
        {
            if (product is null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            await this.context.AddAsync(product);
            await this.context.SaveChangesAsync();
            return product.Id;
        }

        /// <inheritdoc />
        public async Task<bool> DeleteProductAsync(int productId)
        {
            var product = await this.context
                .Products
                .Where(product => productId == product.Id)
                .FirstOrDefaultAsync();
            if (product is null)
            {
                return false;
            }

            this.context
                .Remove(product);
            var updatedRows = await this.context
                .SaveChangesAsync();
            return updatedRows > 0;
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<Product> GetProductsByNameAsync(ICollection<string> names)
        {
            if (names is null)
            {
                throw new ArgumentNullException(nameof(names));
            }

            var productsDto = this.context
                .Products
                .AsNoTracking()
                .Where(product => names.Any(name => name == product.Name))
                .AsAsyncEnumerable();
            await foreach (var productDto in productsDto)
            {
                yield return this.mapper.Map<Product>(productDto);
            }
        }

        /// <inheritdoc />
        public async Task<bool> UpdateProductAsync(int productId, Product product)
        {
            if (product is null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            var updatedProductDto = await this.context
                .Products
                .Where(p => productId == p.Id)
                .FirstOrDefaultAsync();
            if (updatedProductDto is null)
            {
                return false;
            }

            UpdateProductDto(updatedProductDto, product);
            return await this.context.SaveChangesAsync() > 0;
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<Product> GetProductsByCategoryAsync(ICollection<int> categoriesId)
        {
            if (categoriesId is null)
            {
                throw new ArgumentNullException(nameof(categoriesId));
            }

            var productsDto = this.context
                .Products
                .AsNoTracking()
                .Where(product => categoriesId.Any(id => id == (product.CategoryId ?? -1)))
                .AsAsyncEnumerable();
            await foreach (var productDto in productsDto)
            {
                yield return this.mapper.Map<Product>(productDto);
            }
        }

        private static void UpdateProductDto(ProductTransferObject dto, Product product)
        {
            dto.Name = product.Name;
            dto.CategoryId = product.CategoryId;
            dto.Discontinued = product.Discontinued;
            dto.QuantityPerUnit = product.QuantityPerUnit;
            dto.ReorderLevel = product.ReorderLevel;
            dto.SupplierId = product.SupplierId;
            dto.UnitPrice = product.UnitPrice;
            dto.UnitsInStock = product.UnitsInStock;
            dto.UnitsOnOrder = product.UnitsOnOrder;
        }
    }
}
