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
    public class ProductCategoryService : IProductCategoryService
    {
        private readonly NorthwindContext context;
        private readonly IMapper mapper;

        public ProductCategoryService(NorthwindContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<ProductCategory> GetCategoriesAsync(int offset, int limit)
        {
            if (offset < 0)
            {
                throw new ArgumentException("Must be greater than zero or equals zero.", nameof(offset));
            }

            if (limit < 1)
            {
                throw new ArgumentException("Must be greater than zero.", nameof(limit));
            }

            await foreach (var categoryDto in this.context
                               .Categories
                               .AsNoTracking()
                               .Skip(offset)
                               .Take(limit)
                               .AsAsyncEnumerable())
            {
                yield return this.mapper.Map<ProductCategory>(categoryDto);
            }
        }

        /// <inheritdoc />
        public async Task<(bool isSuccess, ProductCategory productCategory)> TryGetCategoryAsync(int categoryId)
        {
            var categoryDto = await this.context
                .Categories
                .AsNoTracking()
                .Where(categoryDto => categoryId == categoryDto.Id)
                .FirstOrDefaultAsync();
            if (categoryDto is null)
            {
                return (false, null);
            }

            return (true, this.mapper.Map<ProductCategory>(categoryDto));
        }

        /// <inheritdoc />
        public async Task<int> CreateCategoryAsync(ProductCategory productCategory)
        {
            ArgumentNullException.ThrowIfNull(productCategory, nameof(productCategory));
            var dto = this.mapper.Map<ProductCategoryTransferObject>(productCategory);
            await this.context
                .Categories
                .AddAsync(dto);
            await this.context.SaveChangesAsync();
            return dto.Id;
        }

        /// <inheritdoc />
        public async Task<bool> DeleteCategoryAsync(int categoryId)
        {
            var dto = await this.context
                .Categories
                .Where(category => category.Id == categoryId)
                .FirstOrDefaultAsync();
            if (dto is null)
            {
                return false;
            }

            this.context
                .Remove(dto);
            var deletedRows = await this.context
                .SaveChangesAsync();
            return deletedRows > 0;
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<ProductCategory> GetCategoriesByNameAsync(ICollection<string> names)
        {
            ArgumentNullException.ThrowIfNull(names, nameof(names));
            await foreach (var dto in this.context
                               .Categories
                               .AsNoTracking()
                               .Where(dto => names.Any(name => name == dto.Name))
                               .AsAsyncEnumerable())
            {
                yield return this.mapper.Map<ProductCategory>(dto);
            }
        }

        /// <inheritdoc />
        public async Task<bool> UpdateCategoriesAsync(int categoryId, ProductCategory productCategory)
        {
            ArgumentNullException.ThrowIfNull(productCategory, nameof(productCategory));
            var categoryDto = await this.context
                .Categories
                .FindAsync(productCategory.Id);
            if (categoryDto is null)
            {
                return false;
            }

            UpdateCategoryDto(categoryDto, productCategory);
            var updatedRows = await this.context.SaveChangesAsync();
            return updatedRows > 0;
        }

        private static void UpdateCategoryDto(ProductCategoryTransferObject categoryDto, ProductCategory productCategory)
        {
            categoryDto.Description = productCategory.Description;
            categoryDto.Name = productCategory.Name;
        }
    }
}
