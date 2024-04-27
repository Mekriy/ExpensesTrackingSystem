using System.Security.Claims;
using EST.BL.Interfaces;
using EST.BL.Services;
using EST.DAL.Models;
using EST.Domain.DTOs;
using EST.Domain.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ETS.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        [HttpGet("{categoryId:Guid}")]
        public async Task<IActionResult> GetCategoryById(
            [FromRoute] Guid categoryId, CancellationToken token)
        {
            if (categoryId == null)
                return BadRequest("No guid");

            var category = await _categoryService.GetById(categoryId, token);
            
            if (category == null)
                return BadRequest();
            return Ok(category);
        }
        [HttpPost]
        public async Task<IActionResult> CreateCategory(
            [FromBody] CreateCategoryDTO categoryDto, CancellationToken token)
        {
            Guid userId;
            try
            {
                userId = Guid.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            }
            catch (Exception e)
            {
                throw new ApiException()
                {
                    StatusCode = StatusCodes.Status422UnprocessableEntity,
                    Title = "Invalid guid",
                    Detail = "Can't parse user guid"
                };
            }
            
            if (!ModelState.IsValid)
                return BadRequest();

            if (string.IsNullOrEmpty(categoryDto.Name))
                return BadRequest("No category");

            return Ok(await _categoryService.Create(categoryDto, userId, token));
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories(CancellationToken token)
        {
            var result = await _categoryService.GetPublic(token);
            if (result.Count > 0)
                return Ok(result);
            throw new ApiException()
            {
                StatusCode = StatusCodes.Status404NotFound,
                Title = "Not found",
                Detail = "There is no categories"
            };
        }
        [HttpGet("users")]
        public async Task<IActionResult> GetUsersCategories(CancellationToken token)
        {
            Guid userId;
            try
            {
                userId = Guid.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            }
            catch (Exception e)
            {
                throw new ApiException()
                {
                    StatusCode = StatusCodes.Status422UnprocessableEntity,
                    Title = "Invalid guid",
                    Detail = "Can't parse user guid"
                };
            }   
            
            var result = await _categoryService.GetUsers(userId, token);
            if (result.Count > 0)
                return Ok(result);
            throw new ApiException()
            {
                StatusCode = StatusCodes.Status404NotFound,
                Title = "Not found",
                Detail = "There is no categories"
            };
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateCategory([FromBody] UpdateCategoryDTO update, CancellationToken token)
        {
            Guid userId;
            try
            {
                userId = Guid.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            }
            catch (Exception e)
            {
                throw new ApiException()
                {
                    StatusCode = StatusCodes.Status422UnprocessableEntity,
                    Title = "Invalid guid",
                    Detail = "Can't parse user guid"
                };
            }

            if (await _categoryService.Update(update, userId, token))
            {
                return NoContent();
            }

            throw new ApiException()
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Title = "Can't update category",
                Detail = "Error occured while updating category on server"
            };
        }
        [HttpDelete("{categoryName}")]
        public async Task<IActionResult> DeleteCategory([FromRoute] string categoryName, CancellationToken token)
        {
            Guid userId;
            try
            {
                userId = Guid.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            }
            catch (Exception e)
            {
                throw new ApiException()
                {
                    StatusCode = StatusCodes.Status422UnprocessableEntity,
                    Title = "Invalid guid",
                    Detail = "Can't parse user guid"
                };
            }
            if (!await _categoryService.Exist(categoryName, userId, token))
                return BadRequest("Category doesn't exist");

            if (await _categoryService.Delete(categoryName, userId, token))
                return NoContent();
            
            throw new ApiException()
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Title = "Can't delete category",
                Detail = "Error occured while deleting category on server"
            };
        }
    }
}
