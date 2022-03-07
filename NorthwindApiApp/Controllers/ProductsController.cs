using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Northwind.Services.Products;

namespace NorthwindApiApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService productService;

        public ProductsController(IProductService productService)
        {
            this.productService = productService ?? throw new ArgumentNullException(nameof(productService));
        }

        [HttpGet("{id}")]

        public async Task<ActionResult<Product>> GetProductById(int id)
        {
            (bool isSuccess, Product product) = await this.productService.TryGetProductAsync(id);
            if (isSuccess)
            {
                return new ObjectResult(product);
            }

            return new NotFoundResult();
        }
    }
}
