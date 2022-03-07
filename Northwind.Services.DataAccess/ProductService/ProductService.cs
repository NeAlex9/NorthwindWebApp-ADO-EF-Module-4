using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Northwind.DataAccess;
using Northwind.DataAccess.Products;
using Northwind.Services.Products;

namespace Northwind.Services.DataAccess.ProductService
{
    /// <summary>
    /// Represent a class that manages products.
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly NorthwindDataAccessFactory factory;
        private readonly IMapper productToProductDTOMapper;

        /// <summary>
        /// Create a new instance of class <see cref="ProductService"/>
        /// </summary>
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
                var productDTO = await this.factory
                    .GetProductDataAccessObject()
                    .FindProduct(productId);
                var product = this.productToProductDTOMapper.Map<Product>(productDTO);
                return (true, product);
            }
            catch (ProductNotFoundException)
            {
                throw;
            }
        }


        /// <inheritdoc />
        public async Task<int> CreateProductAsync(Product product)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public async Task<bool> DeleteProductAsync(int productId)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<Product> GetProductsByNameAsync(IAsyncEnumerable<string> names)
        {
            throw new NotImplementedException();
            yield return null;
        }

        /// <inheritdoc />
        public async Task<bool> UpdateProductAsync(int productId, Product product)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<Product> GetProductsByCategoryAsync(int categoryId)
        {
            throw new NotImplementedException();
            yield return null;
        }
    }
}