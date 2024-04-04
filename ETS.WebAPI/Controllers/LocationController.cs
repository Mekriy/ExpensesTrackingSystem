using EST.BL.Interfaces;
using EST.Domain.DTOs;
using EST.Domain.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace ETS.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly ILocationService _locationService;

        public LocationController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        [HttpPost("{userId:Guid}")]
        public async Task<IActionResult> Create([FromRoute] Guid userId, [FromBody] LocationDTO locationDto)
        {
            var result = await _locationService.Create(locationDto, userId);
            return Ok(result);
        }

        [HttpPost("{expenseId:Guid}/{locationId:Guid}")]
        public async Task<IActionResult> AddLocationToExpense([FromRoute] Guid expenseId, [FromRoute] Guid locationId)
        {
            var result = await _locationService.AddLocationToExpense(expenseId, locationId);
            if (result)
                return Ok();
            else
                throw new ApiException()
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Title = "Can't add location",
                    Detail = "Error occured while adding location to expense"
                };
        }
    }
}