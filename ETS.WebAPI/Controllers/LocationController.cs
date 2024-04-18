using System.Security.Claims;
using EST.BL.Interfaces;
using EST.Domain.DTOs;
using EST.Domain.Helpers;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LocationDTO locationDto)
        {
            Guid userParseId;
            try
            {
                userParseId = Guid.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            }
            catch (Exception e)
            {
                throw new ApiException()
                {
                    StatusCode = StatusCodes.Status422UnprocessableEntity,
                    Title = "Something wrong with user Guid",
                    Detail = "Error occured while parsing guid from user claims"
                };
            }
            var result = await _locationService.Create(locationDto, userParseId);
            return Ok(result);
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            Guid userParseId;
            try
            {
                userParseId = Guid.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            }
            catch (Exception e)
            {
                throw new ApiException()
                {
                    StatusCode = StatusCodes.Status422UnprocessableEntity,
                    Title = "Something wrong with user Guid",
                    Detail = "Error occured while parsing guid from user claims"
                };
            }

            var result = await _locationService.GetUserSavedLocation(userParseId);
            if (result.Count > 0)
                return Ok(result);
            throw new ApiException()
            {
                StatusCode = StatusCodes.Status404NotFound,
                Title = "No locations",
                Detail = "No saved locations"
            };
        }
    }
}