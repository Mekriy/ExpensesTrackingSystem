using EST.BL.Interfaces;
using EST.BL.Services;
using EST.DAL.Models;
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

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllItems()
        {
            var items = await _itemService.GetAll();
            if (items == null || items.Count == 0)
                return BadRequest("There are no items!");
            else
                return Ok(items);
        }
        [HttpGet("GetByGuid")]
        public async Task<IActionResult> GetItemById([FromQuery] Guid itemId)
        {
            var item = await _itemService.GetById(itemId);
            if (item == null)
                return BadRequest("No item");
            return Ok(item);
        }
        [HttpPost("Create")]
        public async Task<IActionResult> CreateItem(Item item)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            if (item == null)
                return BadRequest("No item");

            if (await _itemService.Exist(item.Id))
                return BadRequest("Item already exists");

            if (await _itemService.Create(item))
                return Ok("Item is created");
            else
                return StatusCode(500, "Error occured while creating item on server");
        }
        [HttpPut("Update")]
        public async Task<IActionResult> UpdateItem(Item item)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            if (item == null)
                return BadRequest("No item");

            if (!await _itemService.Exist(item.Id))
                return BadRequest("Item doesn't exist");

            if (await _itemService.Update(item))
                return Ok("Item is updated!");
            else
                return StatusCode(500, "Error occured while updating item on server");
        }
        [HttpDelete("Delete")]
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
