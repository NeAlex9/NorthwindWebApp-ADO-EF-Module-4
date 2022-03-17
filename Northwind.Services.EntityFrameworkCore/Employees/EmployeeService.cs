using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Northwind.DataAccess;
using Northwind.DataAccess.Employees;
using Northwind.Services.Employees;

namespace Northwind.Services.EntityFrameworkCore.Employees
{
    /// <summary>
    /// 
    /// </summary>
    public class EmployeeService : IEmployeeService
    {
        private readonly NorthwindContext context;
        private readonly IMapper mapper;

        public EmployeeService(NorthwindContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<Employee> GetEmployeesAsync(int offset, int limit)
        {
            if (offset < 0)
            {
                throw new ArgumentException("Must be greater than zero or equals zero.", nameof(offset));
            }

            if (limit < 1)
            {
                throw new ArgumentException("Must be greater than zero.", nameof(limit));
            }

            await foreach (var dto in this.context
                               .Employees
                               .AsNoTracking()
                               .Skip(offset)
                               .Take(limit)
                               .AsAsyncEnumerable())
            {
                yield return this.mapper.Map<Employee>(dto);
            }
        }

        /// <inheritdoc />
        public async Task<(bool isSuccess, Employee employee)> TryGetEmployeeIdAsync(int employeeId)
        {
            var dto = await this.context
                .Employees
                .AsNoTracking()
                .Where(dto => dto.Id == employeeId)
                .FirstOrDefaultAsync();
            if (dto is null)
            {
                return (false, null);
            }

            return (true, this.mapper.Map<Employee>(dto));
        }

        /// <inheritdoc />
        public async Task<int> CreateEmployeeAsync(Employee employee)
        {
            ArgumentNullException.ThrowIfNull(employee, nameof(employee));
            var dto = this.mapper.Map<EmployeeTransferObject>(employee);
            await this.context.Employees.AddAsync(dto);
            await this.context.SaveChangesAsync();
            return dto.Id;
        }

        /// <inheritdoc />
        public async Task<bool> DeleteEmployeeAsync(int employeeId)
        {
            var dto = await this.context
                .Employees
                .Where(e => e.Id == employeeId)
                .FirstOrDefaultAsync();
            if (dto is null)
            {
                return false;
            }

            this.context.Remove(dto);
            var deletedRows = await this.context
                .SaveChangesAsync();
            return deletedRows > 0;
        }

        /// <inheritdoc />
        public async Task<bool> UpdateEmployeeAsync(int employeeId, Employee employee)
        {
            ArgumentNullException.ThrowIfNull(employee, nameof(employee));
            var dto = await this.context
                .Employees
                .FindAsync(employee.Id);
            if (dto is null)
            {
                return false;
            }

            UpdateEmployeeDto(dto, employee);
            var updatedRows = await this.context.SaveChangesAsync();
            return updatedRows > 0;
        }

        private static void UpdateEmployeeDto(EmployeeTransferObject dto, Employee employee)
        {
            dto.FirstName = employee.FirstName;
            dto.LastName = employee.LastName;
            dto.Address = employee.Address;
            dto.City = employee.City;
            dto.BirthDate = employee.BirthDate;
            dto.Country = employee.Country;
            dto.Extension = employee.Extension;
            dto.HireDate = employee.HireDate;
            dto.Notes = employee.Notes;
            dto.HomePhone = employee.HomePhone;
            dto.TitleOfCourtesy = employee.TitleOfCourtesy;
            dto.Title = employee.Title;
            dto.Region = employee.Region;
            dto.PostalCode = employee.PostalCode;
            dto.ReportsTo = employee.ReportsTo;
            dto.PhotoPath = employee.PhotoPath;
        }
    }
}