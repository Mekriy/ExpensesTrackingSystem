using System.Security.Claims;
using EST.BL.Interfaces;
using EST.BL.Services;
using EST.DAL.Models;
using EST.Domain.DTOs;
using EST.Domain.Helpers;
using EST.Domain.Helpers.ErrorFilter;
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

        [HttpGet("items")]
        public async Task<IActionResult> GetAllItems(CancellationToken token)
        {
            var items = await _itemService.GetPublicItems(token);
            if (items == null || items.Count == 0)
                return BadRequest("There are no items!");
            else
                return Ok(items);
        }
        [Authorize]
        [HttpGet("user/items")]
        public async Task<IActionResult> GetUserItems(CancellationToken token)
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
            
            var items = await _itemService.GetAllUserItems(userId, token);
            if (items == null || items.Count == 0)
                throw new ApiException()
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Title = "No items",
                    Detail = "There is no items on server"
                };
            else
                return Ok(items);
        }
        [HttpGet("review")]
        public async Task<IActionResult> GetItemsForAdminToReview(CancellationToken token)
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
            var items = await _itemService.GetItemsForAdminToReview(userId, token);
            if (items == null || items.Count == 0)
                throw new ApiException()
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Title = "No items",
                    Detail = "No items for admin to review on server"
                };
            else
                return Ok(items);
        }
        [HttpGet("{itemId:Guid}")]
        public async Task<IActionResult> GetItemById([FromRoute] Guid itemId, CancellationToken token)
        {
            var item = await _itemService.GetById(itemId, token);
            if (item == null)
                throw new ApiException()
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Title = "Item not found",
                    Detail = "Server didn't find item on database"
                };
            return Ok(item);
        }
        [HttpGet("{itemName}")]
        public async Task<IActionResult> GetItemByName([FromRoute] string itemName, CancellationToken token)
        {
            var item = await _itemService.GetByName(itemName, token);
            if (item == null)
                return NotFound("No item");
            return Ok(item);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateItem([FromBody] CreateItemDTO item)
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
        public async Task<IActionResult> UpdateItem([FromRoute] Guid itemId, [FromBody] UpdateItemDTO item)
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
        public async Task<IActionResult> UpdateItemToPublic([FromRoute] Guid adminId, [FromBody] ItemDTO item)
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
        public async Task<IActionResult> DeleteItem([FromQuery] Guid itemId)
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
