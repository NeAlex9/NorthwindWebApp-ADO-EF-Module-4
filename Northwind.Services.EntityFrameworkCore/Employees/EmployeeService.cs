using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Northwind.DataAccess;
using Northwind.Services.Employees;

namespace Northwind.Services.EntityFrameworkCore.Employees
{
    /// <summary>
    /// 
    /// </summary>
    public class EmployeeService : IEmployeeService
    {
        private readonly NorthwindDataAccessFactory factory;
        private readonly IMapper mapper;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="mapper"></param>
        public EmployeeService(NorthwindDataAccessFactory factory, IMapper mapper)
        {
            this.factory = factory;
            this.mapper = mapper;
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
