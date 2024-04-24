using System.Security.Claims;
using EST.BL.Interfaces;
using EST.Domain.DTOs;
using EST.Domain.Helpers;
using EST.Domain.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<IActionResult> GetPrivateItems(
        [FromQuery] PaginationFilter filter, 
        CancellationToken token)
    {
        var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await _adminService.GetItemsForAdminToReview(filter, userId, token);
            return Ok(result);
    }
    [HttpGet("statistic")]
    public async Task<IActionResult> GetStatistic(
        CancellationToken token)
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
    public async Task<IActionResult> GetGeneralInfo(
        CancellationToken token)
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
    public async Task<IActionResult> ChangeItemVisibility(
        [FromBody] ItemToBePublicDTO itemId, CancellationToken token)
    {
        Guid adminId;
        try
        {
            adminId = Guid.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
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
        
        if (await _adminService.ChangeItemsVisibility(itemId, adminId, token))
            return Ok();
        throw new ApiException()
        {
            StatusCode = StatusCodes.Status500InternalServerError,
            Title = "Can't update",
            Detail = "Can't update item's visibility"
        };
    }

    [HttpGet("search")]
    public async Task<IActionResult> GetUserInfo(
        [FromQuery] string query, CancellationToken token)
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
    public async Task<IActionResult> GetUsersCreatedInfo(
        [FromRoute] Guid userId, CancellationToken token)
    {
        var result = await _adminService.GetUsersCreatedInfo(userId, token);
        return Ok(result);
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories(CancellationToken token)
    {
        var result = await _adminService.GetCategories(token);
        return Ok(result);
    }

    [HttpPatch("category")]
    public async Task<IActionResult> UpdateCategory([FromBody] UpdateCategoryDTO update, CancellationToken token)
    {
        if (await _adminService.UpdateCategory(update, token))
            return NoContent();
        throw new ApiException()
        {
            StatusCode = StatusCodes.Status500InternalServerError,
            Title = "Can't update category",
            Detail = "Error occured while updating category"
        };
    }

    [HttpDelete("category/{categoryName}")]
    public async Task<IActionResult> DeleteCategory([FromRoute] string categoryName, CancellationToken token)
    { 
        if (await _adminService.DeleteCategory(categoryName, token))
            return NoContent();
        throw new ApiException()
        {
            StatusCode = StatusCodes.Status500InternalServerError,
            Title = "Can't delete category",
            Detail = "Error occured while deleting category"
        };
    }
}