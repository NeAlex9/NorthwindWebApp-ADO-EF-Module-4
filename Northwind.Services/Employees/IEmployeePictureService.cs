using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Services.Employees
{
    /// <summary>
    /// Represents a management service for category pictures.
    /// </summary>
    public interface IEmployeePictureService
    {
        /// <summary>
        /// Try to show a product category picture.
        /// </summary>
        /// <param name="employeeId">A product category identifier.</param>
        /// <returns>True if a product category is exist; otherwise false.</returns>
        Task<(bool IsSuccess, byte[] imageBytes)> TryGetPictureAsync(int employeeId);

        /// <summary>
        /// Update a product category picture.
        /// </summary>
        /// <param name="employeeId">A product category identifier.</param>
        /// <param name="stream">A <see cref="Stream"/>.</param>
        /// <returns>True if a product category is exist; otherwise false.</returns>
        Task<bool> UpdatePictureAsync(int employeeId, Stream stream);

        /// <summary>
        /// Destroy a product category picture.
        /// </summary>
        /// <param name="employeeId">A product category identifier.</param>
        /// <returns>True if a product category is exist; otherwise false.</returns>
        Task<bool> DeletePictureAsync(int employeeId);
    }
}
