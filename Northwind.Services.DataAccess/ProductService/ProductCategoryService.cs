using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Northwind.DataAccess;
using Northwind.Services.Products;

namespace Northwind.Services.DataAccess.ProductService
{
    /// <summary>
    /// Represent a class that manage product category.
    /// </summary>
    public class ProductCategoryService : IProductCategoryService
    {
        private readonly NorthwindDataAccessFactory factory;
        private readonly IMapper categoryToCategoryDTO;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductCategoryService"/> class.
        /// </summary>
        /// <param name="factory">factory to create dao.</param>
        /// <param name="mapper">Dto mapper.</param>
        public ProductCategoryService(NorthwindDataAccessFactory factory, IMapper mapper)
        {
            this.factory = factory;
            this.categoryToCategoryDTO = mapper;
        }

        /// <inheritdoc />
        public IAsyncEnumerable<ProductCategory> GetCategoriesAsync(int offset, int limit)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<(bool isSuccess, ProductCategory productCategory)> TryGetCategoryAsync(int categoryId)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<int> CreateCategoryAsync(ProductCategory productCategory)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<bool> DeleteCategoryAsync(int categoryId)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public IAsyncEnumerable<ProductCategory> GetCategoriesByNameAsync(IList<string> names)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<bool> UpdateCategoriesAsync(int categoryId, ProductCategory productCategory)
        {
            throw new NotImplementedException();
        }
    }
}
