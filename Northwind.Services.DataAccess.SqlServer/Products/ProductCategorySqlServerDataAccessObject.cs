using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Threading.Tasks;

#pragma warning disable CA2100 // Review SQL queries for security vulnerabilities

namespace Northwind.DataAccess.Products
{
    /// <summary>
    /// Represents a SQL Server-tailored DAO for Northwind product categories.
    /// </summary>
    public sealed class ProductCategorySqlServerDataAccessObject : IProductCategoryDataAccessObject
    {
        private readonly SqlConnection connection;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductCategorySqlServerDataAccessObject"/> class.
        /// </summary>
        /// <param name="connection">A <see cref="SqlConnection"/>.</param>
        public ProductCategorySqlServerDataAccessObject(SqlConnection connection)
        {
            this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        /// <inheritdoc/>
        public async Task<int> InsertProductCategory(ProductCategoryTransferObject productCategory)
        {
            if (productCategory == null)
            {
                throw new ArgumentNullException(nameof(productCategory));
            }

            await using var command = new SqlCommand("InsertCategory", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            AddSqlParameters(productCategory, command);

            await this.connection.OpenAsync();

            var categoryId = (int)await command.ExecuteScalarAsync();
            return categoryId;
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteProductCategory(int productCategoryId)
        {
            if (productCategoryId <= 0)
            {
                throw new ArgumentException("Must be greater than zero.", nameof(productCategoryId));
            }

            await using var command = new SqlCommand("DeleteCategory", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            const string varName = "categoryId";
            command.Parameters.Add(varName, SqlDbType.Int);
            command.Parameters[varName].Value = productCategoryId;

            await this.connection.OpenAsync();

            var deletedRowsCount = await command.ExecuteNonQueryAsync();
            return deletedRowsCount > 0;
        }

        /// <inheritdoc/>
        public async Task<ProductCategoryTransferObject> FindProductCategory(int productCategoryId)
        {
            if (productCategoryId <= 0)
            {
                throw new ArgumentException("Must be greater than zero.", nameof(productCategoryId));
            }

            await using var command = new SqlCommand("FindCategoryById", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            const string varName = "categoryId";
            command.Parameters.Add(varName, SqlDbType.Int);
            command.Parameters[varName].Value = productCategoryId;

            this.connection.Open();
            await using var reader = await command.ExecuteReaderAsync();
            if (!(await reader.ReadAsync()))
            {
                throw new ProductNotFoundException(productCategoryId);
            }

            var category = CreateProductCategory(reader);
            //await this.connection.CloseAsync();
            return category;
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<ProductCategoryTransferObject> SelectProductCategories(int offset, int limit)
        {
            if (offset < 0)
            {
                throw new ArgumentException("Must be greater than zero or equals zero.", nameof(offset));
            }

            if (limit < 1)
            {
                throw new ArgumentException("Must be greater than zero.", nameof(limit));
            }

            await using var command = new SqlCommand("SelectCategories", this.connection)
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
                yield return CreateProductCategory(reader);
            }
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<ProductCategoryTransferObject> SelectProductCategoriesByName(ICollection<string> productCategoryNames)
        {
            if (productCategoryNames == null)
            {
                throw new ArgumentNullException(nameof(productCategoryNames));
            }

            if (productCategoryNames.Count < 1)
            {
                throw new ArgumentException("Collection is empty.", nameof(productCategoryNames));
            }

            await using var command = new SqlCommand("SelectCategoriesByName", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            const string namesVar = "@names";
            command.Parameters.Add(namesVar, SqlDbType.Structured);
            command.Parameters[namesVar].TypeName = "StringCollection";
            command.Parameters[namesVar].Value = CreateDataTable(productCategoryNames, "name");

            await this.connection.OpenAsync();

            var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                yield return CreateProductCategory(reader);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateProductCategory(ProductCategoryTransferObject productCategory)
        {
            if (productCategory == null)
            {
                throw new ArgumentNullException(nameof(productCategory));
            }

            await using var command = new SqlCommand("UpdateCategory", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            const string categoryIdVar = "categoryId";
            command.Parameters.Add(categoryIdVar, SqlDbType.Int);
            command.Parameters[categoryIdVar].Value = productCategory.Id;

            AddSqlParameters(productCategory, command);

            if (this.connection.State == ConnectionState.Closed)
            {
                await this.connection.OpenAsync();
            }

            return (int)await command.ExecuteScalarAsync() > 0;
        }

        private static ProductCategoryTransferObject CreateProductCategory(SqlDataReader reader)
        {
            var id = (int)reader["CategoryID"];
            var name = (string)reader["CategoryName"];

            const string descriptionColumnName = "Description";
            string description = null;

            if (reader[descriptionColumnName] != DBNull.Value)
            {
                description = (string)reader["Description"];
            }

            const string pictureColumnName = "Picture";
            byte[] picture = null;

            if (reader[pictureColumnName] != DBNull.Value)
            {
                picture = (byte[])reader["Picture"];
            }

            return new ProductCategoryTransferObject
            {
                Id = id, Name = name, Description = description, Picture = picture,
            };
        }

        private static void AddSqlParameters(ProductCategoryTransferObject productCategory, SqlCommand command)
        {
            const string categoryNameParameter = "@categoryName";
            command.Parameters.Add(categoryNameParameter, SqlDbType.NVarChar, 15);
            command.Parameters[categoryNameParameter].Value = productCategory.Name;

            const string descriptionParameter = "@description";
            command.Parameters.Add(descriptionParameter, SqlDbType.NText);
            command.Parameters[descriptionParameter].IsNullable = true;

            if (productCategory.Description != null)
            {
                command.Parameters[descriptionParameter].Value = productCategory.Description;
            }
            else
            {
                command.Parameters[descriptionParameter].Value = DBNull.Value;
            }

            const string pictureParameter = "@picture";
            command.Parameters.Add(pictureParameter, SqlDbType.Image);
            command.Parameters[pictureParameter].IsNullable = true;

            if (productCategory.Picture != null)
            {
                command.Parameters[pictureParameter].Value = productCategory.Picture;
            }
            else
            {
                command.Parameters[pictureParameter].Value = DBNull.Value;
            }
        }

        private static DataTable CreateDataTable<T>(IEnumerable<T> collection, string fieldName)
        {
            DataTable table = new DataTable();
            table.Columns.Add(fieldName, typeof(T));
            foreach (T id in collection)
            {
                table.Rows.Add(id);
            }

            return table;
        }
    }
}