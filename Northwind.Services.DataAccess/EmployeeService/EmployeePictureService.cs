namespace Northwind.Services.DataAccess.EmployeeService
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using Northwind.DataAccess;
    using Northwind.DataAccess.Products;
    using Northwind.Services.DataAccess.ProductService;
    using Northwind.Services.Employees;

    /// <summary>
    /// Employee picture service.
    /// </summary>
    public class EmployeePictureService : IEmployeePictureService
    {
        private const int ReservedBytes = 78;
        private readonly NorthwindDataAccessFactory factory;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeePictureService"/> class.
        /// </summary>
        /// <param name="factory">factory.</param>
        /// <param name="categoryToCategoryDto">category DTO mapper.</param>
        public EmployeePictureService(NorthwindDataAccessFactory factory)
        {
            this.factory = factory;
        }

        /// <inheritdoc />
        public async Task<(bool IsSuccess, byte[] imageBytes)> TryGetPictureAsync(int categoryId)
        {
            try
            {
                var dto = await this.factory
                    .GetEmployeeDataAccessObject()
                    .FindEmployeeAsync(categoryId);
                var imageBytes = dto.Photo?[ReservedBytes..];
                return imageBytes is null ? (false, new byte[] { }) : (true, imageBytes);
            }
            catch (ProductCategoryNotFoundException)
            {
                return (false, new byte[] { });
            }
        }

        /// <inheritdoc />
        public async Task<bool> UpdatePictureAsync(int employeeId, Stream stream)
        {
            try
            {
                if (stream is null)
                {
                    throw new NullReferenceException(nameof(stream));
                }

                var dao = this.factory.GetEmployeeDataAccessObject();
                var dto = await dao.FindEmployeeAsync(employeeId);
                if (dto.Photo is null)
                {
                    return false;
                }

                await using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                var bytes = memoryStream.ToArray();
                bytes.CopyTo(dto.Photo, ReservedBytes);

                return await dao.UpdateEmployeeAsync(dto);
            }
            catch (ProductCategoryNotFoundException)
            {
                return false;
            }
        }

        /// <inheritdoc />
        public async Task<bool> DeletePictureAsync(int employeeId)
        {
            try
            {
                var dao = this.factory.GetEmployeeDataAccessObject();
                var dto = await dao.FindEmployeeAsync(employeeId);
                dto.Photo = null;
                return await dao.UpdateEmployeeAsync(dto);
            }
            catch (ProductCategoryNotFoundException)
            {
                return false;
            }
        }
    }
}
