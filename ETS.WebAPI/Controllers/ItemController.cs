using EST.BL.Interfaces;
using EST.BL.Services;
using EST.DAL.Models;
using EST.Domain.DTOs;
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

        [HttpGet]
        public async Task<IActionResult> GetAllItems(CancellationToken token)
        {
            var items = await _itemService.GetAll(token);
            if (items == null || items.Count == 0)
                return BadRequest("There are no items!");
            else
                return Ok(items);
        }
        [HttpGet("review")]
        public async Task<IActionResult> GetItemsToReview(CancellationToken token)
        {
            var items = await _itemService.GetItemsToReview(token);
            if (items == null || items.Count == 0)
                return NotFound("No items to review");
            else
                return Ok(items);
        }
        [HttpGet("{itemId:Guid}")]
        public async Task<IActionResult> GetItemById([FromRoute] Guid itemId, CancellationToken token)
        {
            var item = await _itemService.GetById(itemId, token);
            if (item == null)
                return NotFound("No item");
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
        public async Task<IActionResult> CreateItem([FromBody] ItemDTO item)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            if (item == null)
                return BadRequest("No item");

            if (await _itemService.Exist(item.Name))
                return BadRequest("Item already exists");

            if (await _itemService.Create(item))
                return Ok("Item is created");
            else
                return StatusCode(500, "Error occured while creating item on server");
        }
        [HttpPut]
        public async Task<IActionResult> UpdateItem([FromBody] ItemDTO item)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            if (item == null)
                return BadRequest("No item");

            if (!await _itemService.Exist(item.Name))
                return BadRequest("Item doesn't exist");

            if (await _itemService.Update(item))
                return Ok("Item is updated!");
            else
                return StatusCode(500, "Error occured while updating item on server");
        }
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

            if (await _itemService.Delete(itemId))
                return Ok("Item is deleted!");
            else
                return StatusCode(500, "Error occured while deleting item on server");
        }
    }
}
