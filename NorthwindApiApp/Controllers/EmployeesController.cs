using System;
using Microsoft.AspNetCore.Mvc;
using Northwind.Services.Products;

namespace NorthwindApiApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly IProductCategoryPictureService productCategoryPictureService;

        public EmployeesController(IProductCategoryPictureService productCategoryPictureService)
        {
            this.productCategoryPictureService = productCategoryPictureService ?? throw new ArgumentNullException(nameof(productCategoryPictureService));
        }
    }
}
