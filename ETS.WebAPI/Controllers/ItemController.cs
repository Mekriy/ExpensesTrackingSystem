using System.Security.Claims;
using EST.BL.Interfaces;
using EST.BL.Services;
using EST.DAL.Models;
using EST.Domain.DTOs;
using EST.Domain.Helpers;
using EST.Domain.Helpers.ErrorFilter;
using EST.Domain.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ETS.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly IItemService _itemService;

        public ItemController(IItemService itemService)
        {
            _itemService = itemService;
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllItems(
            [FromQuery] PaginationFilter filter,
            CancellationToken token)
        {
            var userId = "";
            try
            {
                userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
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

            var items = await _itemService.GetItems(filter, userId, token);
            return Ok(items);
        }
        // [Authorize]
        // [HttpGet("user/items")]
        // public async Task<IActionResult> GetUserItems(
        //     [FromQuery] PaginationFilter filter,
        //     CancellationToken token)
        // {
        //     
        //     var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        //     
        //     var items = await _itemService.GetAllUserItems(filter, userId, token);
        //     return Ok(items);
        // }
        [HttpGet("review")]
        public async Task<IActionResult> GetItemsForAdminToReview(
            [FromQuery] PaginationFilter filter,
            CancellationToken token)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var items = await _itemService.GetItemsForAdminToReview(filter, userId, token);
            return Ok(items);
        }
        [HttpGet("{itemId:Guid}")]
        public async Task<IActionResult> GetItemById(
            [FromRoute] Guid itemId,
            CancellationToken token)
        {
            var item = await _itemService.GetById(itemId, token);
            return Ok(item);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateItem(
            [FromBody] CreateItemDTO item)
        {
            if (!ModelState.IsValid)
                return BadRequest();

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
            
            if (item.Name == String.Empty)
                return BadRequest("No item");

            if (await _itemService.Exist(item.Name))
                return BadRequest("Item already exists");

            if (await _itemService.Create(userId, item))
                return Ok("Item is created");
            else
                return StatusCode(500, "Error occured while creating item on server");
        }
        [HttpPut("itemId:Guid")]
        [Authorize]
        public async Task<IActionResult> UpdateItem(
            [FromRoute] Guid itemId,
            [FromBody] UpdateItemDTO item)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            
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
            
            if (item.Name == String.Empty)
                return BadRequest("No item");

            if (!await _itemService.Exist(item.Name))
                return BadRequest("Item doesn't exist");

            if (await _itemService.Update(userId, item, itemId))
                return Ok("Item is updated!");
            else
                return StatusCode(500, "Error occured while updating item on server");
        }
        //TODO: [Authorize(Policy = "Admin")]
        [HttpPut("{adminId:Guid}")]
        public async Task<IActionResult> UpdateItemToPublic(
            [FromRoute] Guid adminId, 
            [FromBody] ItemDTO item)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            if (item == null)
                return BadRequest("No item");

            if (!await _itemService.Exist(item.Name))
                return BadRequest("Item doesn't exist");

            if (await _itemService.UpdateToPublic(adminId, item))
                return Ok("Item is updated!");
            else
                return StatusCode(500, "Error occured while updating item on server");
        }
        [HttpDelete("{itemId:Guid}")]
        public async Task<IActionResult> DeleteItem(
            [FromQuery] Guid itemId)
        {
            if (itemId == Guid.Empty)
                return BadRequest("No guid");

            if (!await _itemService.Exist(itemId))
                return BadRequest("Item doesn't exist");

            if (await _itemService.SoftDelete(itemId))
                return Ok("Item is deleted!");
            else
                return StatusCode(500, "Error occured while deleting item on server");
        }
    }
}
