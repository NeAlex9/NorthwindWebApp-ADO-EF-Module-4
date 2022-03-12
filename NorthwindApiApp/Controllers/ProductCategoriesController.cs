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
    public class CategoriesController : ControllerBase
    {
        private readonly IProductCategoryService productCategoryService;

        public CategoriesController(IProductCategoryService productCategoryService)
        {
            this.productCategoryService = productCategoryService ?? throw new ArgumentNullException(nameof(productCategoryService));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductCategory>> GetCategoryById(int id)
        {
            (bool isSuccess, ProductCategory category) = await this.productCategoryService.TryGetCategoryAsync(id);
            if (isSuccess)
            {
                return new ObjectResult(category);
            }

            return new NotFoundResult();
        }

        [HttpGet]
        public async IAsyncEnumerable<ProductCategory> GetCategories(int offset, int limit)
        {
            await foreach (var category in this.productCategoryService
                               .GetCategoriesAsync(offset, limit))
            {
                yield return category;
            }
        }

        [HttpPost]
        public async Task<ActionResult<ProductCategory>> CreateCategory(ProductCategory category)
        {
            var categoryId = await this.productCategoryService
                .CreateCategoryAsync(category);
            category.Id = categoryId;
            return this.CreatedAtAction(nameof(this.GetCategoryById), new
            {
                id = category.Id
            }, category);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCategory(int id)
        {
            var result = await this.productCategoryService
                .DeleteCategoryAsync(id);
            if (!result)
            {
                return this.NotFound();
            }

            return this.Ok();
        }

        [HttpGet("ByName/{name}")]
        public async IAsyncEnumerable<ProductCategory> GetCategoriesByName(ICollection<string> names)
        {
            await foreach (var category in this.productCategoryService
                               .GetCategoriesByNameAsync(names))
            {
                yield return category;
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCategory(int id, ProductCategory category)
        {
            if (id != category.Id)
            {
                return this.BadRequest();
            }

            var result = await this.productCategoryService
                .UpdateCategoriesAsync(id, category);
            if (!result)
            {
                return this.NotFound();
            }

            return this.Ok();
        }
    }
}