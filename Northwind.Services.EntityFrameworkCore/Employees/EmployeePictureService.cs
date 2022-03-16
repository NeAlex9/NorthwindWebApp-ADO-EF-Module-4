using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Northwind.Services.Employees;

namespace Northwind.Services.EntityFrameworkCore.Employees
{
    public class EmployeePictureService : IEmployeePictureService
    {
        private readonly NorthwindContext context;
        private const int ReservedBytes = 78;

        public EmployeePictureService(NorthwindContext context)
        {
            this.context = context;
        }

        /// <inheritdoc />
        public async Task<(bool IsSuccess, byte[] imageBytes)> TryGetPictureAsync(int employeeId)
        {
            var dto = await this.context
                .Employees
                .AsNoTracking()
                .Where(category => category.Id == employeeId)
                .FirstOrDefaultAsync();
            if (dto is null || dto.Photo is null)
            {
                return (false, new byte[] { });
            }

            return (true, dto.Photo[ReservedBytes..]);
        }

        /// <inheritdoc />
        public async Task<bool> UpdatePictureAsync(int employeeId, Stream stream)
        {
            ArgumentNullException.ThrowIfNull(stream, nameof(stream));
            var dto = await this.context
                .Employees
                .Where(employee => employee.Id == employeeId)
                .FirstOrDefaultAsync();
            if (dto is null)
            {
                return false;
            }

            await using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            var bytes = memoryStream.ToArray();
            bytes.CopyTo(dto.Photo, ReservedBytes);

            var updatedRows = await this.context.SaveChangesAsync();
            return updatedRows > 0;
        }

        /// <inheritdoc />
        public async Task<bool> DeletePictureAsync(int employeeId)
        {
            var dto = await this.context
                .Employees
                .Where(employee => employee.Id == employeeId)
                .FirstOrDefaultAsync();
            if (dto is null)
            {
                return false;
            }

            dto.Photo = null;
            var updatedRows = await this.context.SaveChangesAsync();
            return updatedRows > 0;
        }
    }
}
