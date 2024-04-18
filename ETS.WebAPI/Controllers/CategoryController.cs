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
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        [HttpGet("{categoryId:Guid}")]
        public async Task<IActionResult> GetCategoryById([FromRoute] Guid categoryId, CancellationToken token)
        {
            if (categoryId == null)
                return BadRequest("No guid");

            var category = await _categoryService.GetById(categoryId, token);
            
            if (category == null)
                return BadRequest();
            return Ok(category);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDTO categoryDto)
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

            if (categoryDto.Name == String.Empty)
                return BadRequest("No category");

            return Ok(await _categoryService.Create(categoryDto, userId));
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var result = await _categoryService.GetPublic();
            if (result.Count > 0)
                return Ok(result);
            throw new ApiException()
            {
                StatusCode = StatusCodes.Status404NotFound,
                Title = "Not found",
                Detail = "There is no categories"
            };
        }
        [Authorize]
        [HttpGet("users")]
        public async Task<IActionResult> GetUsersCategories()
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
            
            var result = await _categoryService.GetUsers(userId);
            if (result.Count > 0)
                return Ok(result);
            throw new ApiException()
            {
                StatusCode = StatusCodes.Status404NotFound,
                Title = "Not found",
                Detail = "There is no categories"
            };
        }
        
        [HttpDelete("{categoryId:Guid}")]
        public async Task<IActionResult> DeleteCategory([FromRoute] Guid categoryId)
        {
            if (categoryId == Guid.Empty)
                return BadRequest("No guid");

            if (!await _categoryService.Exist(categoryId))
                return BadRequest("Category doesn't exist");

            if (await _categoryService.Delete(categoryId))
                return Ok("Category is deleted!");
            else
                return StatusCode(500, "Error occured while deleting category on server");
        }
    }
}
