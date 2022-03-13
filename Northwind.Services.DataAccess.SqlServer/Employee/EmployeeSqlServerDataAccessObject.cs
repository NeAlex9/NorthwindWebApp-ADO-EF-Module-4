using System.Threading.Tasks;

namespace Northwind.DataAccess.Employees
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;

    /// <summary>
    /// Represents a SQL Server-tailored DAO for Northwind products.
    /// </summary>
    public sealed class EmployeeSqlServerDataAccessObject : IEmployeeDataAccessObject
    {
        private readonly SqlConnection connection;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeSqlServerDataAccessObject"/> class.
        /// </summary>
        /// <param name="connection">connection.</param>
        public EmployeeSqlServerDataAccessObject(SqlConnection connection)
        {
            this.connection = connection;
        }

        /// <inheritdoc />
        public async Task<EmployeeTransferObject> FindEmployeeAsync(int employeeId)
        {
            if (employeeId <= 0)
            {
                throw new ArgumentException(nameof(employeeId));
            }

            await using var command = new SqlCommand("FindEmployeeById", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            const string employeeVar = "employeeId";
            command.Parameters.Add(employeeVar, SqlDbType.Int);
            command.Parameters[employeeVar].Value = employeeId;

            await this.connection.OpenAsync();

            await using var reader = await command.ExecuteReaderAsync();
            if (!(await reader.ReadAsync()))
            {
                throw new EmployeeNotFoundException(employeeId);
            }

            var employeeDto = CreateEmployee(reader);
            return employeeDto;
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<EmployeeTransferObject> SelectEmployeesAsync(int offset, int limit)
        {
            if (offset < 0)
            {
                throw new ArgumentException("Must be greater than zero or equals zero.", nameof(offset));
            }

            if (limit < 1)
            {
                throw new ArgumentException("Must be greater than zero.", nameof(limit));
            }

            await using var command = new SqlCommand("SelectEmployees", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            const string offsetVar = "offset";
            command.Parameters.Add(offsetVar, SqlDbType.Int);
            command.Parameters[offsetVar].Value = offset;

            const string limitVar = "limit";
            command.Parameters.Add(limitVar, SqlDbType.Int);
            command.Parameters[limitVar].Value = limit;

            await this.connection.OpenAsync();

            var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                yield return CreateEmployee(reader);
            }
        }

        /// <inheritdoc />
        public async Task<int> InsertEmployeeAsync(EmployeeTransferObject employee)
        {
            if (employee == null)
            {
                throw new ArgumentNullException(nameof(employee));
            }

            await using var command = new SqlCommand("InsertEmployee", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            AddSqlParameters(employee, command);

            await this.connection.OpenAsync();

            var id = (int)await command.ExecuteScalarAsync();
            return id;
        }

        /// <inheritdoc />
        public async Task<bool> DeleteEmployeeAsync(int employeeId)
        {
            if (employeeId <= 0)
            {
                throw new ArgumentException("Must be greater than zero.", nameof(employeeId));
            }

            await using var command = new SqlCommand("DeleteEmployee", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            const string varName = "employeeId";
            command.Parameters.Add(varName, SqlDbType.Int);
            command.Parameters[varName].Value = employeeId;

            await this.connection.OpenAsync();

            var deletedRowsCount = await command.ExecuteNonQueryAsync();
            return deletedRowsCount > 0;
        }

        /// <inheritdoc />
        public async Task<bool> UpdateEmployeeAsync(EmployeeTransferObject employee)
        {
            if (employee == null)
            {
                throw new ArgumentNullException(nameof(employee));
            }

            await using var command = new SqlCommand("UpdateEmployee", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            const string categoryIdVar = "employeeId";
            command.Parameters.Add(categoryIdVar, SqlDbType.Int);
            command.Parameters[categoryIdVar].Value = employee.Id;

            AddSqlParameters(employee, command);

            if (this.connection.State != ConnectionState.Open)
            {
                await this.connection.OpenAsync();
            }

            return (int)await command.ExecuteScalarAsync() > 0;
        }

        private static EmployeeTransferObject CreateEmployee(SqlDataReader reader)
        {
            var id = (int)reader.GetValue(0);
            var lastName = reader.GetString(1);
            var firstName = reader.GetString(2);
            string title = null;
            if (reader.GetValue(3) is not DBNull)
            {
                title = reader.GetString(3);
            }

            string titleOfCourtesy = null;
            if (reader.GetValue(4) is not DBNull)
            {
                titleOfCourtesy = reader.GetString(4);
            }

            DateTime? birthDate = null;
            if (reader.GetValue(5) is not DBNull)
            {
                birthDate = reader.GetDateTime(5);
            }

            DateTime? hireDate = null;
            if (reader.GetValue(6) is not DBNull)
            {
                hireDate = reader.GetDateTime(6);
            }

            var address = reader.GetString(7);
            if (reader.GetValue(7) is not DBNull)
            {
                address = reader.GetString(7);
            }

            var city = reader.GetString(8);
            if (reader.GetValue(8) is not DBNull)
            {
                city = reader.GetString(8);
            }

            string region = null;
            if (reader.GetValue(9) is not DBNull)
            {
                region = reader.GetString(9);
            }

            string postalCode = null;
            if (reader.GetValue(10) is not DBNull)
            {
                postalCode = reader.GetString(10);
            }

            string country = null;
            if (reader.GetValue(11) is not DBNull)
            {
                country = reader.GetString(11);
            }

            string homePhone = null;
            if (reader.GetValue(12) is not DBNull)
            {
                homePhone = reader.GetString(12);
            }

            string extension = null;
            if (reader.GetValue(13) is not DBNull)
            {
                extension = reader.GetString(13);
            }

            byte[] photo = null;
            if (reader.GetValue(14) is not DBNull)
            {
                photo = (byte[])reader.GetValue(14);
            }

            string notes = null;
            if (reader.GetValue(15) is not DBNull)
            {
                notes = reader.GetString(15);
            }

            int? reportsTo = null;
            if (reader.GetValue(16) is not DBNull)
            {
                reportsTo = reader.GetInt32(16);
            }

            string photoPath = null;
            if (reader.GetValue(17) is not DBNull)
            {
                photoPath = reader.GetString(17);
            }

            return new EmployeeTransferObject
            {
                Id = id,
                FirstName = firstName,
                LastName = lastName,
                Title = title,
                TitleOfCourtesy = titleOfCourtesy,
                BirthDate = birthDate,
                HireDate = hireDate,
                Address = address,
                City = city,
                Region = region,
                PostalCode = postalCode,
                Country = country,
                HomePhone = homePhone,
                Extension = extension,
                Photo = photo,
                Notes = notes,
                ReportsTo = reportsTo,
                PhotoPath = photoPath,
            };
        }

        private static void AddSqlParameters(EmployeeTransferObject employee, SqlCommand command)
        {
            const string lastNameParameter = "@lastName";
            command.Parameters.Add(lastNameParameter, SqlDbType.NVarChar, 20);
            command.Parameters[lastNameParameter].Value = employee.LastName;

            const string firstNameParameter = "@firstName";
            command.Parameters.Add(firstNameParameter, SqlDbType.NVarChar, 10);
            command.Parameters[firstNameParameter].Value = employee.FirstName;

            const string titleParameter = "@title";
            command.Parameters.Add(titleParameter, SqlDbType.NVarChar, 30);
            command.Parameters[titleParameter].IsNullable = true;
            if (employee.Title != null)
            {
                command.Parameters[titleParameter].Value = employee.Title;
            }
            else
            {
                command.Parameters[titleParameter].Value = DBNull.Value;
            }

            const string titleOfCourtesyParameter = "@titleOfCourtesy";
            command.Parameters.Add(titleOfCourtesyParameter, SqlDbType.NVarChar, 25);
            command.Parameters[titleOfCourtesyParameter].IsNullable = true;
            if (employee.TitleOfCourtesy != null)
            {
                command.Parameters[titleOfCourtesyParameter].Value = employee.TitleOfCourtesy;
            }
            else
            {
                command.Parameters[titleOfCourtesyParameter].Value = DBNull.Value;
            }

            const string birthDateParameter = "@birthDate";
            command.Parameters.Add(birthDateParameter, SqlDbType.DateTime);
            command.Parameters[birthDateParameter].IsNullable = true;
            if (employee.BirthDate != null)
            {
                command.Parameters[birthDateParameter].Value = employee.BirthDate;
            }
            else
            {
                command.Parameters[birthDateParameter].Value = DBNull.Value;
            }

            const string hireDateParameter = "@hireDate";
            command.Parameters.Add(hireDateParameter, SqlDbType.DateTime);
            command.Parameters[hireDateParameter].IsNullable = true;
            if (employee.HireDate != null)
            {
                command.Parameters[hireDateParameter].Value = employee.HireDate;
            }
            else
            {
                command.Parameters[hireDateParameter].Value = DBNull.Value;
            }

            const string addressParameter = "@address";
            command.Parameters.Add(addressParameter, SqlDbType.NVarChar, 60);
            command.Parameters[addressParameter].IsNullable = true;
            if (employee.Address != null)
            {
                command.Parameters[addressParameter].Value = employee.Address;
            }
            else
            {
                command.Parameters[addressParameter].Value = DBNull.Value;
            }

            const string cityParameter = "@city";
            command.Parameters.Add(cityParameter, SqlDbType.NVarChar, 15);
            command.Parameters[cityParameter].IsNullable = true;
            if (employee.City != null)
            {
                command.Parameters[cityParameter].Value = employee.City;
            }
            else
            {
                command.Parameters[cityParameter].Value = DBNull.Value;
            }

            const string regionParameter = "@region";
            command.Parameters.Add(regionParameter, SqlDbType.NVarChar, 15);
            command.Parameters[regionParameter].IsNullable = true;
            if (employee.Region != null)
            {
                command.Parameters[regionParameter].Value = employee.Region;
            }
            else
            {
                command.Parameters[regionParameter].Value = DBNull.Value;
            }

            const string postalCodeParameter = "@postalCode";
            command.Parameters.Add(postalCodeParameter, SqlDbType.NVarChar, 10);
            command.Parameters[postalCodeParameter].IsNullable = true;
            if (employee.PostalCode != null)
            {
                command.Parameters[postalCodeParameter].Value = employee.PostalCode;
            }
            else
            {
                command.Parameters[postalCodeParameter].Value = DBNull.Value;
            }

            const string countryParameter = "@country";
            command.Parameters.Add(countryParameter, SqlDbType.NVarChar, 15);
            command.Parameters[countryParameter].IsNullable = true;
            if (employee.Country != null)
            {
                command.Parameters[countryParameter].Value = employee.Country;
            }
            else
            {
                command.Parameters[countryParameter].Value = DBNull.Value;
            }

            const string homePhoneParameter = "@homePhone";
            command.Parameters.Add(homePhoneParameter, SqlDbType.NVarChar, 24);
            command.Parameters[homePhoneParameter].IsNullable = true;
            if (employee.HomePhone != null)
            {
                command.Parameters[homePhoneParameter].Value = employee.HomePhone;
            }
            else
            {
                command.Parameters[homePhoneParameter].Value = DBNull.Value;
            }

            const string extensionParameter = "@extension";
            command.Parameters.Add(extensionParameter, SqlDbType.NVarChar, 4);
            command.Parameters[extensionParameter].IsNullable = true;
            if (employee.Extension != null)
            {
                command.Parameters[extensionParameter].Value = employee.Extension;
            }
            else
            {
                command.Parameters[extensionParameter].Value = DBNull.Value;
            }

            const string photoParameter = "@photo";
            command.Parameters.Add(photoParameter, SqlDbType.Image);
            command.Parameters[photoParameter].IsNullable = true;
            if (employee.Photo != null)
            {
                command.Parameters[photoParameter].Value = employee.Photo;
            }
            else
            {
                command.Parameters[photoParameter].Value = DBNull.Value;
            }

            const string notesParameter = "@notes";
            command.Parameters.Add(notesParameter, SqlDbType.NText);
            command.Parameters[notesParameter].IsNullable = true;
            if (employee.Notes != null)
            {
                command.Parameters[notesParameter].Value = employee.Notes;
            }
            else
            {
                command.Parameters[notesParameter].Value = DBNull.Value;
            }

            const string reportsToParameter = "@reportsTo";
            command.Parameters.Add(reportsToParameter, SqlDbType.Int);
            command.Parameters[reportsToParameter].IsNullable = true;
            if (employee.ReportsTo != null)
            {
                command.Parameters[reportsToParameter].Value = employee.Notes;
            }
            else
            {
                command.Parameters[reportsToParameter].Value = DBNull.Value;
            }

            const string photoPathParameter = "@photoPath";
            command.Parameters.Add(photoPathParameter, SqlDbType.NVarChar, 255);
            command.Parameters[photoPathParameter].IsNullable = true;
            if (employee.PhotoPath != null)
            {
                command.Parameters[photoPathParameter].Value = employee.PhotoPath;
            }
            else
            {
                command.Parameters[photoPathParameter].Value = DBNull.Value;
            }
        }
    }
}
