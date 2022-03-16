// <copyright file="ProductService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Northwind.Services.DataAccess.ProductService
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using AutoMapper;
    using Northwind.DataAccess;
    using Northwind.DataAccess.Products;
    using Northwind.Services.Products;

    /// <summary>
    /// Represent a class that manages products.
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly NorthwindDataAccessFactory factory;
        private readonly IMapper productToProductDTOMapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductService"/> class.
        /// </summary>
        /// <param name="mapper">mapper.</param>
        /// <param name="factory">factory to create dao.</param>
        public ProductService(NorthwindDataAccessFactory factory, IMapper mapper)
        {
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
            this.productToProductDTOMapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<Product> GetProductsAsync(int offset, int limit)
        {
            await foreach (var product in this.factory
                               .GetProductDataAccessObject()
                               .SelectProducts(offset, limit))
            {
                yield return this.productToProductDTOMapper.Map<Product>(product);
            }
        }

        /// <inheritdoc />
        public async Task<(bool, Product)> TryGetProductAsync(int productId)
        {
            try
            {
                var productDto = await this.factory
                    .GetProductDataAccessObject()
                    .FindProduct(productId);
                var product = this.productToProductDTOMapper.Map<Product>(productDto);
                return (true, product);
            }
            catch (ProductNotFoundException)
            {
                return (false, null);
            }
        }

        /// <inheritdoc />
        public async Task<int> CreateProductAsync(Product product) =>
            await this.factory
                .GetProductDataAccessObject()
                .InsertProduct(this.productToProductDTOMapper.Map<ProductTransferObject>(product));

        /// <inheritdoc />
        public async Task<bool> DeleteProductAsync(int productId) =>
            await this.factory
                .GetProductDataAccessObject()
                .DeleteProduct(productId);

        /// <inheritdoc />
        public async IAsyncEnumerable<Product> GetProductsByNameAsync(ICollection<string> names)
        {
            await foreach (var product in this.factory
                              .GetProductDataAccessObject()
                              .SelectProductsByName(names))
            {
                yield return this.productToProductDTOMapper.Map<Product>(product);
            }
        }

        /// <inheritdoc />
        public async Task<bool> UpdateProductAsync(int productId, Product product)
        {
            product.Id = productId;
            var result = await this.factory
                .GetProductDataAccessObject()
                .UpdateProduct(this.productToProductDTOMapper.Map<ProductTransferObject>(product));
            return result;
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<Product> GetProductsByCategoryAsync(ICollection<int> categoriesId)
        {
            await foreach (var product in this.factory
                               .GetProductDataAccessObject()
                               .SelectProductByCategory(categoriesId))
            {
                yield return this.productToProductDTOMapper.Map<Product>(product);
            }
        }
    }
}