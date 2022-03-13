using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;
using Northwind.DataAccess.Products;

#pragma warning disable CA1032 // Implement standard exception constructors

namespace Northwind.DataAccess.Employees
{
    /// <summary>
    /// Employee exception.
    /// </summary>
    [Serializable]
    public class EmployeeNotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeNotFoundException"/> class with specified identifier and object type.
        /// </summary>
        /// <param name="id">A requested identifier.</param>
        public EmployeeNotFoundException(int id)
            : base(string.Format(CultureInfo.InvariantCulture, $"A employee with identifier = {id}."))
        {
            this.ProductCategoryId = id;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeNotFoundException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The object that holds the serialized object data.</param>
        /// <param name="context">An object that describes the source or destination of the serialized data.</param>
        protected EmployeeNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Gets an identifier of a employee that is missed in a data storage.
        /// </summary>
        public int ProductCategoryId { get; }
    }
}
