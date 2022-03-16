using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Northwind.Services.Products;

namespace NorthwindApiApp.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService productService;

        public ProductsController(IProductService productService)
        {
            this.productService = productService ?? throw new ArgumentNullException(nameof(productService));
        }

        [HttpGet("{id}")]
        [ApiConventionMethod(typeof(DefaultApiConventions),
            nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<Product>> GetProductById(int id)
        {
            (bool isSuccess, Product product) = await this.productService.TryGetProductAsync(id);
            if (isSuccess)
            {
                return new ObjectResult(product);
            }

            return this.NotFound();
        }

        [HttpGet()]
        [ApiConventionMethod(typeof(DefaultApiConventions),
            nameof(DefaultApiConventions.Get))]
        public async IAsyncEnumerable<Product> GetProduct(int offset, int limit)
        {
            await foreach (var product in this.productService
                               .GetProductsAsync(offset, limit))
            {
                yield return product;
            }
        }

        [HttpPost]
        [ApiConventionMethod(typeof(DefaultApiConventions),
            nameof(DefaultApiConventions.Post))]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            var productId = await this.productService.CreateProductAsync(product);
            product.Id = productId;
            return this.CreatedAtAction(nameof(this.GetProductById), new
            {
                id = product.Id
            }, product);
        }

        [HttpDelete("{id}")]
        [ApiConventionMethod(typeof(DefaultApiConventions),
            nameof(DefaultApiConventions.Delete))]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var result = await this.productService.DeleteProductAsync(id);
            if (!result)
            {
                return this.NotFound();
            }

            return this.Ok();
        }

        [HttpGet("ByNames")]
        [ApiConventionMethod(typeof(DefaultApiConventions),
            nameof(DefaultApiConventions.Get))]
        public async IAsyncEnumerable<Product> GetProductsByName([FromQuery] ICollection<string> names)
        {
            await foreach (var product in this.productService
                               .GetProductsByNameAsync(names))
            {
                yield return product;
            }
        }

        [HttpGet("ByCategory")]
        [ApiConventionMethod(typeof(DefaultApiConventions),
            nameof(DefaultApiConventions.Get))]
        public async IAsyncEnumerable<Product> GetProductsByCategory([FromQuery] ICollection<int> categories)
        {
            await foreach (var product in this.productService
                               .GetProductsByCategoryAsync(categories))
            {
                yield return product;
            }
        }

        [HttpPut]
        [ApiConventionMethod(typeof(DefaultApiConventions),
            nameof(DefaultApiConventions.Put))]
        public async Task<IActionResult> UpdateProduct(int productId, Product product)
        {
            product.Id = productId;
            var result = await this.productService.UpdateProductAsync(productId, product);
            if (!result)
            {
                return this.NotFound();
            }

            return this.Ok();
        }
    }
}