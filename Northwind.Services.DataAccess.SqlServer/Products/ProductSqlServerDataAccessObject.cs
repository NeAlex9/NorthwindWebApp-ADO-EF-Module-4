using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

#pragma warning disable CA2100 // Review SQL queries for security vulnerabilities

namespace Northwind.DataAccess.Products
{
    /// <summary>
    /// Represents a SQL Server-tailored DAO for Northwind products.
    /// </summary>
    public sealed class ProductSqlServerDataAccessObject : IProductDataAccessObject
    {
        private readonly SqlConnection connection;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductSqlServerDataAccessObject"/> class.
        /// </summary>
        /// <param name="connection">A <see cref="SqlConnection"/>.</param>
        public ProductSqlServerDataAccessObject(SqlConnection connection)
        {
            this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        /// <inheritdoc/>
        public async Task<int> InsertProduct(ProductTransferObject product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            await using var command = new SqlCommand("InsertProduct", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };
            AddSqlParameters(product, command);
            await this.connection.OpenAsync();
            var result = (int) await command.ExecuteScalarAsync();
            return result;
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteProduct(int productId)
        {
            if (productId <= 0)
            {
                throw new ArgumentException("Must be greater than zero.", nameof(productId));
            }

            await using var command = new SqlCommand("DeleteProduct", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            const string productNameParameter = "@productId";
            command.Parameters.Add(productNameParameter, SqlDbType.Int);
            command.Parameters[productNameParameter].Value = productId;

            await this.connection.OpenAsync();
            var row = await command.ExecuteNonQueryAsync();
            return row > 0;
        }

        /// <inheritdoc/>
        public async Task<ProductTransferObject> FindProduct(int productId)
        {
            if (productId <= 0)
            {
                throw new ArgumentException("Must be greater than zero.", nameof(productId));
            }

            await using var command = new SqlCommand("FindProductById", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            const string productIdParameter = "@productId";
            command.Parameters.Add(productIdParameter, SqlDbType.Int);
            command.Parameters[productIdParameter].Value = productId;

            await this.connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();

            if (!(await reader.ReadAsync()))
            {
                throw new ProductNotFoundException(productId);
            }

            return CreateProduct(reader);
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<ProductTransferObject> SelectProducts(int offset, int limit)
        {
            if (offset < 0)
            {
                throw new ArgumentException("Must be greater than zero or equals zero.", nameof(offset));
            }

            if (limit < 1)
            {
                throw new ArgumentException("Must be greater than zero.", nameof(limit));
            }

            await using var command = new SqlCommand("SelectProducts", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            const string offsetVar = "@offset";
            command.Parameters.Add(offsetVar, SqlDbType.Int);
            command.Parameters[offsetVar].Value = offset;

            const string limitVar = "@limit";
            command.Parameters.Add(limitVar, SqlDbType.Int);
            command.Parameters[limitVar].Value = limit;

            await this.connection.OpenAsync();

            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                yield return CreateProduct(reader);
            }
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<ProductTransferObject> SelectProductsByName(ICollection<string> productNames)
        {
            if (productNames == null)
            {
                throw new ArgumentNullException(nameof(productNames));
            }

            if (productNames.Count < 1)
            {
                throw new ArgumentException("Collection is empty.", nameof(productNames));
            }

            await using var command = new SqlCommand("SelectProductsByName", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            const string namesVar = "@names";
            command.Parameters.Add(namesVar, SqlDbType.Structured);
            command.Parameters[namesVar].TypeName = "StringCollection";
            command.Parameters[namesVar].Value = productNames;

            await this.connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                yield return CreateProduct(reader);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateProduct(ProductTransferObject product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            await using var command = new SqlCommand("UpdateProduct", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            const string productNameParameter = "@productId";
            command.Parameters.Add(productNameParameter, SqlDbType.Int);
            command.Parameters[productNameParameter].Value = product.Id;

            AddSqlParameters(product, command);

            await this.connection.OpenAsync();

            var result = await command.ExecuteNonQueryAsync();
            return result > 0;
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<ProductTransferObject> SelectProductByCategory(ICollection<int> collectionOfCategoryId)
        {
            if (collectionOfCategoryId == null)
            {
                throw new ArgumentNullException(nameof(collectionOfCategoryId));
            }

            await using var command = new SqlCommand("SelectProductsByCategory", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            const string namesVar = "@categories";
            command.Parameters.Add(namesVar, SqlDbType.Structured);
            command.Parameters[namesVar].TypeName = "IntCollection";
            command.Parameters[namesVar].Value = collectionOfCategoryId;

            await this.connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                yield return CreateProduct(reader);
            }
        }

        private static ProductTransferObject CreateProduct(SqlDataReader reader)
        {
            var id = (int)reader["ProductID"];
            var name = (string)reader["ProductName"];

            const string supplierIdColumnName = "SupplierID";
            int? supplierId;

            if (reader[supplierIdColumnName] != DBNull.Value)
            {
                supplierId = (int)reader[supplierIdColumnName];
            }
            else
            {
                supplierId = null;
            }

            const string categoryIdColumnName = "CategoryID";
            int? categoryId;

            if (reader[categoryIdColumnName] != DBNull.Value)
            {
                categoryId = (int)reader[categoryIdColumnName];
            }
            else
            {
                categoryId = null;
            }

            const string quantityPerUnitColumnName = "QuantityPerUnit";
            string quantityPerUnit;

            if (reader[quantityPerUnitColumnName] != DBNull.Value)
            {
                quantityPerUnit = (string)reader[quantityPerUnitColumnName];
            }
            else
            {
                quantityPerUnit = null;
            }

            const string unitPriceColumnName = "UnitPrice";
            decimal? unitPrice;

            if (reader[unitPriceColumnName] != DBNull.Value)
            {
                unitPrice = (decimal)reader[unitPriceColumnName];
            }
            else
            {
                unitPrice = null;
            }

            const string unitsInStockColumnName = "UnitsInStock";
            short? unitsInStock;

            if (reader[unitsInStockColumnName] != DBNull.Value)
            {
                unitsInStock = (short)reader[unitsInStockColumnName];
            }
            else
            {
                unitsInStock = null;
            }

            const string unitsOnOrderColumnName = "UnitsOnOrder";
            short? unitsOnOrder;

            if (reader[unitsOnOrderColumnName] != DBNull.Value)
            {
                unitsOnOrder = (short)reader[unitsOnOrderColumnName];
            }
            else
            {
                unitsOnOrder = null;
            }

            const string reorderLevelColumnName = "ReorderLevel";
            short? reorderLevel;

            if (reader[reorderLevelColumnName] != DBNull.Value)
            {
                reorderLevel = (short)reader[reorderLevelColumnName];
            }
            else
            {
                reorderLevel = null;
            }

            const string discontinuedColumnName = "Discontinued";
            bool discontinued = (bool)reader[discontinuedColumnName];

            return new ProductTransferObject
            {
                Id = id,
                Name = name,
                SupplierId = supplierId,
                CategoryId = categoryId,
                QuantityPerUnit = quantityPerUnit,
                UnitPrice = unitPrice,
                UnitsInStock = unitsInStock,
                UnitsOnOrder = unitsOnOrder,
                ReorderLevel = reorderLevel,
                Discontinued = discontinued,
            };
        }

        private static void AddSqlParameters(ProductTransferObject product, SqlCommand command)
        {
            const string productNameParameter = "@productName";
            command.Parameters.Add(productNameParameter, SqlDbType.NVarChar, 40);
            command.Parameters[productNameParameter].Value = product.Name;

            const string supplierIdParameter = "@supplierId";
            command.Parameters.Add(supplierIdParameter, SqlDbType.Int);
            command.Parameters[supplierIdParameter].IsNullable = true;

            if (product.SupplierId != null)
            {
                command.Parameters[supplierIdParameter].Value = product.SupplierId;
            }
            else
            {
                command.Parameters[supplierIdParameter].Value = DBNull.Value;
            }

            const string categoryIdParameter = "@categoryId";
            command.Parameters.Add(categoryIdParameter, SqlDbType.Int);
            command.Parameters[categoryIdParameter].IsNullable = true;

            if (product.CategoryId != null)
            {
                command.Parameters[categoryIdParameter].Value = product.CategoryId;
            }
            else
            {
                command.Parameters[categoryIdParameter].Value = DBNull.Value;
            }

            const string quantityPerUnitParameter = "@quantityPerUnit";
            command.Parameters.Add(quantityPerUnitParameter, SqlDbType.NVarChar, 20);
            command.Parameters[quantityPerUnitParameter].IsNullable = true;

            if (product.QuantityPerUnit != null)
            {
                command.Parameters[quantityPerUnitParameter].Value = product.QuantityPerUnit;
            }
            else
            {
                command.Parameters[quantityPerUnitParameter].Value = DBNull.Value;
            }

            const string unitPriceParameter = "@unitPrice";
            command.Parameters.Add(unitPriceParameter, SqlDbType.Money);
            command.Parameters[unitPriceParameter].IsNullable = true;

            if (product.UnitPrice != null)
            {
                command.Parameters[unitPriceParameter].Value = product.UnitPrice;
            }
            else
            {
                command.Parameters[unitPriceParameter].Value = DBNull.Value;
            }

            const string unitsInStockParameter = "@unitsInStock";
            command.Parameters.Add(unitsInStockParameter, SqlDbType.SmallInt);
            command.Parameters[unitsInStockParameter].IsNullable = true;

            if (product.UnitsInStock != null)
            {
                command.Parameters[unitsInStockParameter].Value = product.UnitsInStock;
            }
            else
            {
                command.Parameters[unitsInStockParameter].Value = DBNull.Value;
            }

            const string unitsOnOrderParameter = "@unitsOnOrder";
            command.Parameters.Add(unitsOnOrderParameter, SqlDbType.SmallInt);
            command.Parameters[unitsOnOrderParameter].IsNullable = true;

            if (product.UnitsOnOrder != null)
            {
                command.Parameters[unitsOnOrderParameter].Value = product.UnitsOnOrder;
            }
            else
            {
                command.Parameters[unitsOnOrderParameter].Value = DBNull.Value;
            }

            const string reorderLevelParameter = "@reorderLevel";
            command.Parameters.Add(reorderLevelParameter, SqlDbType.SmallInt);
            command.Parameters[reorderLevelParameter].IsNullable = true;

            if (product.ReorderLevel != null)
            {
                command.Parameters[reorderLevelParameter].Value = product.ReorderLevel;
            }
            else
            {
                command.Parameters[reorderLevelParameter].Value = DBNull.Value;
            }

            const string discontinuedParameter = "@discontinued";
            command.Parameters.Add(discontinuedParameter, SqlDbType.Bit);
            command.Parameters[discontinuedParameter].Value = product.Discontinued;
        }

        private async IAsyncEnumerable<ProductTransferObject> ExecuteReader(string commandText)
        {
            await using var command = new SqlCommand(commandText, this.connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                yield return CreateProduct(reader);
            }
        }
    }
}