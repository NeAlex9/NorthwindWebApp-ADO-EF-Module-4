// <copyright file="MapperProfile.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Northwind.Services.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using AutoMapper;
    using Northwind.DataAccess.Employees;
    using Northwind.DataAccess.Products;
    using Northwind.Services.Employees;
    using Northwind.Services.Products;

    /// <summary>
    /// Mapper profile.
    /// </summary>
    public class MapperProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MapperProfile"/> class.
        /// </summary>
        public MapperProfile()
        {
            this.CreateMap<Product, ProductTransferObject>();
            this.CreateMap<ProductTransferObject, Product>();
            this.CreateMap<ProductCategory, ProductCategoryTransferObject>();
            this.CreateMap<ProductCategoryTransferObject, ProductCategory>();
            this.CreateMap<Employee, EmployeeTransferObject>();
            this.CreateMap<EmployeeTransferObject, Employee>();
        }
    }
}
