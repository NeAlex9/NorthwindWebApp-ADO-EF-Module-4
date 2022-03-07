using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Northwind.Services.Products;

namespace NorthwindApiApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductCategoriesController : ControllerBase
    {
        private readonly IProductCategoryService productCategoryService;

        public ProductCategoriesController(IProductCategoryService productCategoryService)
        {
            this.productCategoryService = productCategoryService ?? throw new ArgumentNullException(nameof(productCategoryService));
        }

        [HttpGet("{id}")]

        public async Task<ActionResult<ProductCategory>> GetProductById(int id)
        {
            (bool isSuccess, ProductCategory category) = await this.productCategoryService.TryGetCategoryAsync(id);
            if (isSuccess)
            {
                return new ObjectResult(category);
            }

            return new NotFoundResult();
        }
    }
}
