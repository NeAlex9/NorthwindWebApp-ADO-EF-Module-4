using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Northwind.DataAccess;
using Northwind.DataAccess.Employees;
using Northwind.DataAccess.Products;
using Northwind.Services.DataAccess;
using Northwind.Services.DataAccess.ProductService;
using Northwind.Services.Employees;
using Northwind.Services.EntityFrameworkCore.Employees;
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
            services.AddControllers()
                .AddJsonOptions(option => option.JsonSerializerOptions.WriteIndented = true);
            services.AddSwaggerGen();
            services.AddTransient<IProductService, ProductService>()
                .AddTransient<IProductCategoryService, ProductCategoryService>()
                .AddTransient<IProductCategoryPictureService, ProductCategoryPictureService>()
                .AddTransient<IEmployeeService, EmployeeService>()
                .AddTransient<IMapper, Mapper>(provider => new Mapper(new MapperConfiguration(config => config.AddProfile(new MapperProfile()))))
                .AddTransient<NorthwindDataAccessFactory, SqlServerDataAccessFactory>()
                .AddTransient(e => new SqlConnection(this.Configuration.GetConnectionString("SqlConnection")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI((app) =>
                {
                    app.SwaggerEndpoint("/swagger/v1/swagger.json", "My");
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