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
                return NoContent();
            throw new ApiException()
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Title = "Can't create item",
                Detail = "Can't create item on server"
            };
        }
        [HttpPut("{itemId:Guid}")]
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

            if (!await _itemService.Exist(itemId))
                return BadRequest("Item doesn't exist");

            if (await _itemService.Update(userId, item, itemId))
                return NoContent();
            throw new ApiException()
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Title = "Can't update item",
                Detail = "Error occured while updating item on server"
            };
        }
        [HttpDelete("{itemId:Guid}")]
        public async Task<IActionResult> DeleteItem(
            [FromRoute] Guid itemId)
        {
            if (itemId == Guid.Empty)
                return BadRequest("No guid");

            if (!await _itemService.Exist(itemId))
                return BadRequest("Item doesn't exist");

            if (await _itemService.SoftDelete(itemId))
                return NoContent();
            throw new ApiException()
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Title = "Can't delete item",
                Detail = "Error occured while deleting item on server"
            };
        }
    }
}
