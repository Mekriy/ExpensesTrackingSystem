﻿using EST.BL.Interfaces;
using EST.BL.Services;
using EST.DAL.Models;
using EST.Domain.DTOs;
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
        public async Task<IActionResult> CreateCategory([FromBody] CategoryDTO categoryDto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            if (categoryDto == null)
                return BadRequest("No category");

            if (await _categoryService.Create(categoryDto))
                return Ok("Category is created");
            else
                return StatusCode(500, "Error occured while creating category on server");
        }
        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateCategoryDTO categoryDto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            if (categoryDto == null)
                return BadRequest("No category");

            if (categoryDto.UserId == Guid.Empty)
                return BadRequest("No user guid! Cannot update!");

            if (!await _categoryService.Exist(categoryDto.OldName))
                return BadRequest("Category for update doesn't exist");

            if (await _categoryService.Update(categoryDto))
                return Ok("Category is updated!");
            else
                return StatusCode(500, "Error occured while updating category on server");
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
