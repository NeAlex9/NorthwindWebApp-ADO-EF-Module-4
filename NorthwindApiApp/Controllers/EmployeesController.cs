using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Northwind.Services.DataAccess.EmployeeService;
using Northwind.Services.Employees;
using Northwind.Services.Products;

namespace NorthwindApiApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService employeeService;
        private readonly IEmployeePictureService pictureService;

        public EmployeesController(IEmployeeService employeeService, IEmployeePictureService pictureService)
        {
            this.employeeService = employeeService ?? throw new ArgumentNullException(nameof(employeeService));
            this.pictureService = pictureService ?? throw new ArgumentNullException(nameof(pictureService));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductCategory>> GetCategoryById(int id)
        {
            (bool isSuccess, Employee employee) = await this.employeeService.TryGetEmployeeIdAsync(id);
            if (isSuccess)
            {
                return new ObjectResult(employee);
            }

            return new NotFoundResult();
        }

        [HttpGet]
        public async IAsyncEnumerable<Employee> GetEmployee(int offset, int limit)
        {
            await foreach (var employee in this.employeeService
                               .GetEmployeesAsync(offset, limit))
            {
                yield return employee;
            }
        }

        [HttpPost]
        public async Task<ActionResult<Employee>> CreateEmployee(Employee employee)
        {
            var categoryId = await this.employeeService
                .CreateEmployeeAsync(employee);
            employee.Id = categoryId;
            return this.CreatedAtAction(nameof(this.GetCategoryById), new
            {
                id = employee.Id
            }, employee);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteEmployee(int id)
        {
            var result = await this.employeeService
                .DeleteEmployeeAsync(id);
            if (!result)
            {
                return this.NotFound();
            }

            return this.Ok();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateEmployee(int id, Employee employee)
        {
            if (id != employee.Id)
            {
                return this.BadRequest();
            }

            var result = await this.employeeService
                .UpdateEmployeeAsync(id, employee);
            if (!result)
            {
                return this.NotFound();
            }

            return this.Ok();
        }

        [HttpPut("{employeeId}/picture")]
        public async Task<IActionResult> UpdateCategoryPicture(int employeeId, IFormFile formFile)
        {
            var result = await this.pictureService
                .UpdatePictureAsync(employeeId, formFile?.OpenReadStream());
            if (!result)
            {
                return this.NotFound();
            }

            return this.Ok();
        }

        [HttpGet("{employeeId}/picture")]
        public async Task<IActionResult> GetCategoryPicture(int employeeId)
        {
            var (result, imageBytes) = await this.pictureService
                .TryGetPictureAsync(employeeId);
            if (!result)
            {
                return this.NotFound();
            }

            return this.File(imageBytes, "image/bmp");
        }

        [HttpDelete("{employeeId}/picture")]
        public async Task<IActionResult> DeleteCategoryPicture(int employeeId)
        {
            var result = await this.pictureService.DeletePictureAsync(employeeId);
            if (!result)
            {
                return this.NotFound();
            }

            return this.Ok();
        }
    }
}
