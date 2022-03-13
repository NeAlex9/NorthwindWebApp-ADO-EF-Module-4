using System.Collections.Generic;
using System.Threading.Tasks;

#pragma warning disable CA1040

namespace Northwind.DataAccess.Employees
{
    /// <summary>
    /// Represents a DAO for Northwind employees.
    /// </summary>
    public interface IEmployeeDataAccessObject
    {
        /// <summary>
        /// Finds a employee by special identifier.
        /// </summary>
        /// <param name="employeeId">employee id.</param>
        /// <returns>employee dto.</returns>
        public Task<EmployeeTransferObject> FindEmployeeAsync(int employeeId);

        /// <summary>
        /// Gets collection of employees with offset and limit.
        /// </summary>
        /// <param name="offset">offset.</param>
        /// <param name="limit">limit.</param>
        /// <returns>collection of employees dto.</returns>
        public IAsyncEnumerable<EmployeeTransferObject> SelectEmployeesAsync(int offset, int limit);

        /// <summary>
        /// Inserts employee.
        /// </summary>
        /// <param name="employeeId">id.</param>
        /// <returns>true or false.</returns>
        public Task<int> InsertEmployeeAsync(EmployeeTransferObject employee);

        /// <summary>
        /// Deletes employee by id.
        /// </summary>
        /// <param name="employeeId">id.</param>
        /// <returns>true or false.</returns>
        public Task<bool> DeleteEmployeeAsync(int employeeId);

        /// <summary>
        /// Updates employee by id.
        /// </summary>
        /// <param name="employeeId">id.</param>
        /// <returns>true or false.</returns>
        public Task<bool> UpdateEmployeeAsync(EmployeeTransferObject employee);
    }
}
