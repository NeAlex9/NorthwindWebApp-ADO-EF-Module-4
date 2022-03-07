using System;
using System.Collections.Generic;
using System.Text;
using Northwind.DataAccess;
using Northwind.DataAccess.Employees;
using Northwind.DataAccess.Products;

namespace Northwind.Services
{
    /// <summary>
    /// Represents sql factory.
    /// </summary>
    public class SqlDataAccessFactory : NorthwindDataAccessFactory
    {
        private readonly IProductCategoryDataAccessObject productCategoryDataAccess;
        private readonly IProductDataAccessObject productDataAccessObject;
        private readonly IEmployeeDataAccessObject employeeDataAccessObject;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlDataAccessFactory"/> class.
        /// </summary>
        /// <param name="productCategoryDataAccess">Product category dao.</param>
        /// <param name="productDataAccessObject">Product dao.</param>
        /// <param name="employeeDataAccessObject">Employee dao.</param>
        public SqlDataAccessFactory(IProductCategoryDataAccessObject productCategoryDataAccess, IProductDataAccessObject productDataAccessObject, IEmployeeDataAccessObject employeeDataAccessObject)
        {
            this.productCategoryDataAccess = productCategoryDataAccess;
            this.productDataAccessObject = productDataAccessObject;
            this.employeeDataAccessObject = employeeDataAccessObject;
        }

        /// <inheritdoc />
        public override IProductDataAccessObject GetProductDataAccessObject() =>
            this.productDataAccessObject;

        /// <inheritdoc />
        public override IProductCategoryDataAccessObject GetProductCategoryDataAccessObject() =>
            this.productCategoryDataAccess;

        /// <inheritdoc />
        public override IEmployeeDataAccessObject GetEmployeeDataAccessObject() =>
            this.employeeDataAccessObject;
    }
}
