using System.Collections.Generic;
using System.Threading.Tasks;
using Northwind.Services.Products;

#pragma warning disable CA1040

namespace Northwind.Services.Employees
{
    /// <summary>
    /// Represents a management service for employees.
    /// </summary>
    public interface IEmployeeService
    {
        /// <summary>
        /// Shows a list of employees using specified offset and limit for pagination.
        /// </summary>
        /// <param name="offset">An offset of the first element to return.</param>
        /// <param name="limit">A limit of elements to return.</param>
        /// <returns>A <see cref="IAsyncEnumerable{T}"/> of <see cref="Employee"/>.</returns>
        IAsyncEnumerable<Employee> GetEmployeesAsync(int offset, int limit);

        /// <summary>
        /// Try to show a employee with specified identifier.
        /// </summary>
        /// <param name="employeeId">A product identifier.</param>
        /// <returns>Returns true if a product is returned; otherwise false.</returns>
        Task<(bool isSuccess, Employee employee)> TryGetEmployeeIdAsync(int employeeId);

        /// <summary>
        /// Creates a new employee.
        /// </summary>
        /// <param name="employee">A <see cref="Employee"/> to create.</param>
        /// <returns>An identifier of a created product.</returns>
        Task<int> CreateEmployeeAsync(Employee employee);

        /// <summary>
        /// Destroys an existed employee.
        /// </summary>
        /// <param name="employeeId">A product identifier.</param>
        /// <returns>True if a product is destroyed; otherwise false.</returns>
        Task<bool> DeleteEmployeeAsync(int employeeId);

        /// <summary>
        /// Updates a employee.
        /// </summary>
        /// <param name="employeeId">A product identifier.</param>
        /// <param name="employee">A <see cref="Employee"/>.</param>
        /// <returns>True if a product is updated; otherwise false.</returns>
        Task<bool> UpdateEmployeeAsync(int employeeId, Employee employee);
    }
}
