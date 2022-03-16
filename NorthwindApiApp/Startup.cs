using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Data.SqlClient;
using AutoMapper;
using Northwind.DataAccess;
using Northwind.Services.DataAccess;
using Northwind.Services.Employees;
using Northwind.Services.EntityFrameworkCore;
using Northwind.Services.Products;

namespace NorthwindApiApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddControllers()
                .AddJsonOptions(option => option.JsonSerializerOptions.WriteIndented = true);
            services
                .AddSwaggerGen();
            services
                .AddTransient<IProductService, Northwind.Services.EntityFrameworkCore.Products.ProductService>()
                .AddTransient<IEmployeePictureService, Northwind.Services.EntityFrameworkCore.Employees.EmployeePictureService>()
                .AddTransient<IProductCategoryService, Northwind.Services.EntityFrameworkCore.Products.ProductCategoryService>()
                .AddTransient<IProductCategoryPictureService, Northwind.Services.EntityFrameworkCore.Products.ProductCategoryPictureService>()
                .AddTransient<IEmployeeService, Northwind.Services.EntityFrameworkCore.Employees.EmployeeService>()

/*                .AddTransient<IProductService, ProductService>()
                .AddTransient<IEmployeePictureService, EmployeePictureService>()
                .AddTransient<IProductCategoryService, ProductCategoryService>()
                .AddTransient<IProductCategoryPictureService, ProductCategoryPictureService>()
                .AddTransient<IEmployeeService, EmployeeService>()*/

                .AddTransient<IMapper, Mapper>(_ => new Mapper(new MapperConfiguration(config => config.AddProfile(new MapperProfile()))))
                .AddTransient<NorthwindDataAccessFactory, SqlServerDataAccessFactory>()
                .AddTransient(_ => new NorthwindContext(this.Configuration.GetConnectionString("SqlConnection")))
                .AddTransient(_ => new SqlConnection(this.Configuration.GetConnectionString("SqlConnection")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI((app) =>
                {
                    app.SwaggerEndpoint("/swagger/v1/swagger.json", "Custom");
                });
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}