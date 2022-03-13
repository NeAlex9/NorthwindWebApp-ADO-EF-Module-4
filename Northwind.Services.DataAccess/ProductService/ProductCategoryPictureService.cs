﻿using System;
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
            this.factory = factory;
        }

        /// <inheritdoc />
        public async Task<(bool IsSuccess, byte[] imageBytes)> TryGetPictureAsync(int categoryId)
        {
            try
            {
                var dto = await this.factory
                    .GetProductCategoryDataAccessObject()
                    .FindProductCategory(categoryId);
                var imageBytes = dto.Picture[ReservedBytes..];
                return (true, imageBytes);
            }
            catch (ProductCategoryNotFoundException)
            {
                return (false, null);
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
                dto.Picture = null;
                await using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                var bytes = memoryStream.ToArray();
                if (dto.Picture is null)
                {
                    dto.Picture = bytes;
                }
                else
                {
                    bytes.CopyTo(dto.Picture, ReservedBytes);
                }

                return await dao.UpdateProductCategory(dto);
            }
            catch (ProductCategoryNotFoundException)
            {
                return false;
            }
            catch (NullReferenceException)
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
            catch (NullReferenceException)
            {
                return false;
            }
        }
    }
}
