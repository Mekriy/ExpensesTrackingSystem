using System.Security.Claims;
using EST.BL.Interfaces;
using EST.Domain.DTOs;
using EST.Domain.Helpers;
using EST.Domain.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ETS.WebAPI.Controllers;

[Route("api/[controller]")]
[Authorize(Policy = "RequireAdminRole")]
[ApiController]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;
    private readonly IExpenseService _expenseService;

    public AdminController(IAdminService adminService, IExpenseService expenseService)
    {
        _adminService = adminService;
        _expenseService = expenseService;
    }
    [HttpGet]
    public async Task<IActionResult> GetPrivateItems([FromQuery] PaginationFilter filter, CancellationToken token)
    {
        var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await _adminService.GetItemsForAdminToReview(filter, userId, token);
            return Ok(result);
    }
    [HttpGet("statistic")]
    public async Task<IActionResult> GetStatistic(CancellationToken token)
    {
        var result = await _adminService.GetStatistic(token);
        if(result.Count > 0)
           return Ok(result);
        throw new ApiException()
        {
            StatusCode = StatusCodes.Status404NotFound,
            Title = "No statistic",
            Detail = "No statistic found on db for today"
        };
    }
    [HttpGet("general-info")]
    public async Task<IActionResult> GetGeneralInfo(CancellationToken token)
    {
        var result = await _adminService.GetGeneralInfo(token);
        if(result.Count > 0)
            return Ok(result);
        throw new ApiException()
        {
            StatusCode = StatusCodes.Status404NotFound,
            Title = "No general info",
            Detail = "No general info found on db for today"
        };
    }

    [HttpPatch]
    public async Task<IActionResult> ChangeItemVisibility([FromBody] ItemDTO itemId, CancellationToken token)
    {
        if (await _adminService.ChangeItemsVisibility(itemId, token))
            return Ok();
        throw new ApiException()
        {
            StatusCode = StatusCodes.Status500InternalServerError,
            Title = "Can't update",
            Detail = "Can't update item's visibility"
        };
    }

    [HttpGet("search")]
    public async Task<IActionResult> GetUserInfo([FromQuery] string query, CancellationToken token)
    {
        var result = await _adminService.GetUsersByQuery(query, token);
        return Ok(result);
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUsersExpenses(
        [FromRoute] string userId, 
        [FromQuery] PaginationFilter filterRequest, 
        CancellationToken token)
    {
        var result = await _expenseService.GetAllUserExpenses(filterRequest, userId, token);
        return Ok(result);
    }

    [HttpGet("users/{userId:Guid}")]
    public async Task<IActionResult> GetUsersCreatedInfo([FromRoute] Guid userId, CancellationToken token)
    {
        var result = await _adminService.GetUsersCreatedInfo(userId, token);
        return Ok(result);
    }
}