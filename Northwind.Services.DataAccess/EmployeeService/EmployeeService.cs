using System;

namespace Northwind.Services.DataAccess.EmployeeService
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoMapper;
    using Northwind.DataAccess;
    using Northwind.DataAccess.Employees;
    using Northwind.Services.Employees;

    /// <summary>
    /// Employee service class.
    /// </summary>
    public class EmployeeService : IEmployeeService
    {
        private readonly NorthwindDataAccessFactory factory;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeService"/> class.
        /// </summary>
        /// <param name="factory">factory.</param>
        /// <param name="employeeToEmployeeDto">employee dto.</param>
        public EmployeeService(NorthwindDataAccessFactory factory, IMapper employeeToEmployeeDto)
        {
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
            this.mapper = employeeToEmployeeDto ?? throw new ArgumentNullException(nameof(this.mapper));
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<Employee> GetEmployeesAsync(int offset, int limit)
        {
            await foreach (var employee in this.factory
                               .GetEmployeeDataAccessObject()
                               .SelectEmployeesAsync(offset, limit))
            {
                yield return this.mapper.Map<Employee>(employee);
            }
        }

        /// <inheritdoc />
        public async Task<(bool isSuccess, Employee employee)> TryGetEmployeeIdAsync(int employeeId)
        {
            try
            {
                var employeeDto = await this.factory
                    .GetEmployeeDataAccessObject()
                    .FindEmployeeAsync(employeeId);
                return (true, this.mapper.Map<Employee>(employeeDto));
            }
            catch (EmployeeNotFoundException)
            {
                return (false, null);
            }
        }

        /// <inheritdoc />
        public async Task<int> CreateEmployeeAsync(Employee employee)
        {
            var result = await this.factory
                .GetEmployeeDataAccessObject()
                .InsertEmployeeAsync(this.mapper.Map<EmployeeTransferObject>(employee));
            return result;
        }

        /// <inheritdoc />
        public async Task<bool> DeleteEmployeeAsync(int employeeId)
        {
            return await this.factory
                .GetEmployeeDataAccessObject()
                .DeleteEmployeeAsync(employeeId);
        }

        /// <inheritdoc />
        public async Task<bool> UpdateEmployeeAsync(int employeeId, Employee employee)
        {
            return await this.factory
                .GetEmployeeDataAccessObject()
                .UpdateEmployeeAsync(this.mapper.Map<EmployeeTransferObject>(employee));
        }
    }
}
