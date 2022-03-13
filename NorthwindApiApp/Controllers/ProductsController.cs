﻿using System;
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
        public async IAsyncEnumerable<Product> GetProduct(int offset, int limit)
        {
            await foreach (var product in this.productService
                               .GetProductsAsync(offset, limit))
            {
                yield return product;
            }
        }

        [HttpPost]
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
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var result = await this.productService.DeleteProductAsync(id);
            if (!result)
            {
                return this.NotFound();
            }

            return this.Ok();
        }

        [HttpGet("ByNames/{names}")]
        public async IAsyncEnumerable<Product> GetProductsByName([FromBody] ICollection<string> names)
        {
            await foreach (var product in this.productService
                               .GetProductsByNameAsync(names))
            {
                yield return product;
            }
        }

        [HttpGet("ByCategory")]
        public async IAsyncEnumerable<Product> GetProductsByCategory([FromQuery]ICollection<int> categories)
        {
            await foreach (var product in this.productService
                               .GetProductsByCategoryAsync(categories))
            {
                yield return product;
            }
        }

        [HttpPut]
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