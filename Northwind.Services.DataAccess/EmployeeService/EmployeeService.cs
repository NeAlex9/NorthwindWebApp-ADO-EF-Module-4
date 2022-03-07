using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Northwind.DataAccess.Employees;
using Northwind.Services.Employees;

namespace Northwind.Services.DataAccess.EmployeeService
{
    /// <summary>
    /// Employee service class.
    /// </summary>
    public class EmployeeService : IEmployeeService
    {
        private readonly SqlDataAccessFactory factory;
        private readonly IMapper employeeToEmployeeDTO;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeService"/> class.
        /// </summary>
        /// <param name="factory">factory.</param>
        /// <param name="employeeToEmployeeDto">employee dto.</param>
        public EmployeeService(SqlDataAccessFactory factory, IMapper employeeToEmployeeDto)
        {
            this.factory = factory;
            this.employeeToEmployeeDTO = employeeToEmployeeDto;
        }

        /// <inheritdoc />
        public IAsyncEnumerable<Employee> GetEmployeesAsync(int offset, int limit)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<(bool isSuccess, Employee product)> TryGetEmployeeIdAsync(int employeeId)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<int> CreateEmployeeIdAsync(Employee employee)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<bool> DeleteEmployeeAsync(int employeeId)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<bool> UpdateEmployeeAsync(int employeeId, Employee employee)
        {
            throw new NotImplementedException();
        }
    }
}
