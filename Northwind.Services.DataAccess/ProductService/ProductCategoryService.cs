// <copyright file="ProductCategoryService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Northwind.Services.DataAccess.ProductService
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using AutoMapper;
    using Northwind.DataAccess;
    using Northwind.DataAccess.Products;
    using Northwind.Services.Products;

    /// <summary>
    /// Represent a class that manage product category.
    /// </summary>
    public class ProductCategoryService : IProductCategoryService
    {
        private readonly NorthwindDataAccessFactory factory;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductCategoryService"/> class.
        /// </summary>
        /// <param name="factory">factory to create dao.</param>
        /// <param name="mapper">Dto mapper.</param>
        public ProductCategoryService(NorthwindDataAccessFactory factory, IMapper mapper)
        {
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<ProductCategory> GetCategoriesAsync(int offset, int limit)
        {
            await foreach (var category in this.factory
                               .GetProductCategoryDataAccessObject()
                               .SelectProductCategories(offset, limit))
            {
                yield return this.mapper.Map<ProductCategory>(category);
            }
        }

        /// <inheritdoc />
        public async Task<(bool isSuccess, ProductCategory productCategory)> TryGetCategoryAsync(int categoryId)
        {
            try
            {
                var dto = await this.factory
                    .GetProductCategoryDataAccessObject()
                    .FindProductCategory(categoryId);
                var product = this.mapper.Map<ProductCategory>(dto);
                return (true, product);
            }
            catch (ProductNotFoundException)
            {
                return (false, null);
            }
        }

        /// <inheritdoc />
        public async Task<int> CreateCategoryAsync(ProductCategory productCategory)
        {
            ArgumentNullException.ThrowIfNull(productCategory, nameof(productCategory));
            return await this.factory
                .GetProductCategoryDataAccessObject()
                .InsertProductCategory(this.mapper.Map<ProductCategoryTransferObject>(productCategory));
        }

        /// <inheritdoc />
        public async Task<bool> DeleteCategoryAsync(int categoryId)
        {
            return await this.factory
                .GetProductCategoryDataAccessObject()
                .DeleteProductCategory(categoryId);
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<ProductCategory> GetCategoriesByNameAsync(ICollection<string> names)
        {
            ArgumentNullException.ThrowIfNull(names, nameof(names));
            await foreach (var dto in this.factory
                               .GetProductCategoryDataAccessObject()
                               .SelectProductCategoriesByName(names))
            {
                yield return this.mapper.Map<ProductCategory>(dto);
            }
        }

        /// <inheritdoc />
        public async Task<bool> UpdateCategoriesAsync(int categoryId, ProductCategory productCategory)
        {
            ArgumentNullException.ThrowIfNull(productCategory, nameof(productCategory));
            productCategory.Id = categoryId;
            return await this.factory
                .GetProductCategoryDataAccessObject()
                .UpdateProductCategory(this.mapper.Map<ProductCategoryTransferObject>(productCategory));
        }
    }
}