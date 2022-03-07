using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Text;

namespace Northwind.Services.EntityFrameworkCore
{
    public class NorthwindContext : DbContext
    {
        private readonly DbContext context;

        protected NorthwindContext(DbContext context)
        {
            this.context = context;
        }


    }
}