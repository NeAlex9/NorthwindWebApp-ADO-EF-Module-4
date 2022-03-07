using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Northwind.DataAccess;
using Northwind.Services.Products;

namespace Northwind.Services.DataAccess.ProductService
{
    /// <summary>
    /// Category picture service.
    /// </summary>
    public class ProductCategoryPictureService : IProductCategoryPictureService
    {
        private readonly NorthwindDataAccessFactory factory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductCategoryPictureService"/> class.
        /// </summary>
        /// <param name="factory">factory.</param>
        /// <param name="categoryToCategoryDto">category DTO mapper.</param>
        public ProductCategoryPictureService(NorthwindDataAccessFactory factory)
        {
            this.factory = factory;
        }

        /// <inheritdoc />
        public Task<bool> TryGetPictureAsync(int categoryId)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<bool> UpdatePictureAsync(int categoryId, Stream stream)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<(bool isSuccess, byte[] bytes)> DeletePictureAsync(int categoryId)
        {
            throw new NotImplementedException();
        }
    }
}
