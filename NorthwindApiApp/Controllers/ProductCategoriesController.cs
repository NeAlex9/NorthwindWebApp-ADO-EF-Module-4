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
        private readonly IProductCategoryService categoryService;
        private readonly IProductCategoryPictureService pictureService;

        public CategoriesController(IProductCategoryService categoryService, IProductCategoryPictureService pictureService)
        {
            this.categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
            this.pictureService = pictureService ?? throw new ArgumentNullException(nameof(pictureService));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductCategory>> GetCategoryById(int id)
        {
            (bool isSuccess, ProductCategory category) = await this.categoryService.TryGetCategoryAsync(id);
            if (isSuccess)
            {
                return new ObjectResult(category);
            }

            return new NotFoundResult();
        }

        [HttpGet]
        public async IAsyncEnumerable<ProductCategory> GetCategories(int offset, int limit)
        {
            await foreach (var category in this.categoryService
                               .GetCategoriesAsync(offset, limit))
            {
                yield return category;
            }
        }

        [HttpPost]
        public async Task<ActionResult<ProductCategory>> CreateCategory(ProductCategory category)
        {
            var categoryId = await this.categoryService
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
            var result = await this.categoryService
                .DeleteCategoryAsync(id);
            if (!result)
            {
                return this.NotFound();
            }

            return this.Ok();
        }

        [HttpGet("ByName")]
        public async IAsyncEnumerable<ProductCategory> GetCategoriesByName([FromQuery]ICollection<string> names)
        {
            await foreach (var category in this.categoryService
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

            var result = await this.categoryService
                .UpdateCategoriesAsync(id, category);
            if (!result)
            {
                return this.NotFound();
            }

            return this.Ok();
        }

        [HttpPut("{categoryId}/picture")]
        public async Task<IActionResult> UpdateCategoryPicture(int categoryId, IFormFile formFile)
        {
            var result = await this.pictureService
                .UpdatePictureAsync(categoryId, formFile?.OpenReadStream());
            if (!result)
            {
                return this.NotFound();
            }

            return this.Ok();
        }

        [HttpGet("{categoryId}/picture")]
        public async Task<IActionResult> GetCategoryPicture(int categoryId)
        {
            var (result, imageBytes) = await this.pictureService
                .TryGetPictureAsync(categoryId);
            if (!result)
            {
                return this.NotFound();
            }

            return this.File(imageBytes, "image/bmp");
        }

        [HttpDelete("{categoryId}/picture")]
        public async Task<IActionResult> DeleteCategoryPicture(int categoryId)
        {
            var result = await this.pictureService.DeletePictureAsync(categoryId);
            if (!result)
            {
                return this.NotFound();
            }

            return this.Ok();
        }
    }
}