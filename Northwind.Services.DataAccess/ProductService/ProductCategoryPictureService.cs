// <copyright file="ProductCategoryPictureService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Northwind.Services.DataAccess.ProductService
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Northwind.DataAccess;
    using Northwind.DataAccess.Products;
    using Northwind.Services.Products;

    /// <summary>
    /// Category picture service.
    /// </summary>
    public class ProductCategoryPictureService : IProductCategoryPictureService
    {
        private const int ReservedBytes = 78;
        private readonly NorthwindDataAccessFactory factory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductCategoryPictureService"/> class.
        /// </summary>
        /// <param name="factory">factory.</param>
        /// <param name="categoryToCategoryDto">category DTO mapper.</param>
        public ProductCategoryPictureService(NorthwindDataAccessFactory factory)
        {
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        /// <inheritdoc />
        public async Task<(bool IsSuccess, byte[] imageBytes)> TryGetPictureAsync(int categoryId)
        {
            try
            {
                var dto = await this.factory
                    .GetProductCategoryDataAccessObject()
                    .FindProductCategory(categoryId);
                if (dto.Picture is null)
                {
                    return (false, new byte[] { });
                }

                var imageBytes = dto.Picture[ReservedBytes..];
                return (true, imageBytes);
            }
            catch (ProductCategoryNotFoundException)
            {
                return (false, new byte[] { });
            }
        }

        /// <inheritdoc />
        public async Task<bool> UpdatePictureAsync(int categoryId, Stream stream)
        {
            try
            {
                if (stream is null)
                {
                    throw new NullReferenceException(nameof(stream));
                }

                var dao = this.factory.GetProductCategoryDataAccessObject();
                var dto = await dao.FindProductCategory(categoryId);
                if (dto.Picture is null)
                {
                    return false;
                }

                await using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                var bytes = memoryStream.ToArray();
                bytes.CopyTo(dto.Picture, ReservedBytes);
                return await dao.UpdateProductCategory(dto);
            }
            catch (ProductCategoryNotFoundException)
            {
                return false;
            }
        }

        /// <inheritdoc />
        public async Task<bool> DeletePictureAsync(int categoryId)
        {
            try
            {
                var dao = this.factory.GetProductCategoryDataAccessObject();
                var dto = await dao.FindProductCategory(categoryId);
                dto.Picture = null;
                return await dao.UpdateProductCategory(dto);
            }
            catch (ProductCategoryNotFoundException)
            {
                return false;
            }
        }
    }
}
