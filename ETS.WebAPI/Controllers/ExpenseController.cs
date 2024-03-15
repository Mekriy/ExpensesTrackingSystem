using System.Security.Claims;
using EST.BL.Interfaces;
using EST.DAL.Models;
using EST.Domain.DTOs;
using EST.Domain.Helpers;
using EST.Domain.Helpers.ErrorFilter;
using EST.Domain.Pagination;
using Microsoft.AspNetCore.Mvc;

namespace ETS.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExpenseController : ControllerBase
    {
        private readonly IExpenseService _expenseService;
        public ExpenseController(IExpenseService expenseService)
        {
            _expenseService = expenseService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int pageNumber,
            [FromQuery] int pageSize,
            [FromQuery] string sortColumn,
            [FromQuery] string sortDirection,
            CancellationToken token)
        {
            var filter = new PaginationFilter(pageNumber, pageSize, sortColumn, sortDirection);
            var expenses = await _expenseService.GetAll(filter, token);
            return Ok(expenses);
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUserExpensesPagination(
            [FromQuery] int pageNumber,
            [FromQuery] int pageSize,
            [FromQuery] string sortColumn,
            [FromQuery] string sortDirection,
            CancellationToken token)
        {
            var user = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var filter = new PaginationFilter(pageNumber, pageSize, sortColumn, sortDirection);
            var expenses = await _expenseService.GetAllUserExpenses(filter,user, token);
            return Ok(expenses);
        }
        [HttpGet("{expenseId:Guid}")]
        public async Task<IActionResult> GetExpensesById([FromQuery] Guid expenseId, CancellationToken token)
        {
            var expense = await _expenseService.GetById(expenseId, token);
            if (expense == null)
                return BadRequest("No expense!");
            return Ok(expense);
        }
        [HttpPost]
        public async Task<IActionResult> CreateExpense(ExpenseCreateDTO expense, CancellationToken token)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            if (expense == null)
                return BadRequest("No expense!");
            
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

            expense.UserId = userParseId;
            var createdExpense = await _expenseService.Create(expense, token);
            if (createdExpense != null)
                return Ok(createdExpense);
            else
                throw new ApiException()
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Title = "Can't create expense",
                    Detail = "Error occured while creating expense on server"
                };
        }
        [HttpPut]
        public async Task<IActionResult> UpdateExpense([FromBody] ExpenseUpdateDTO expense)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            
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
            
            if (expense == null)
                return BadRequest("No expense");

            var result = await _expenseService.Update(expense, userParseId);
            if (result != null)
                return Ok(result);
            else
                throw new ApiException()
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Title = "Can't update expense",
                    Detail = "Error occured while updating expense on server"
                };
        }
        [HttpDelete("{expenseId:Guid}")]
        public async Task<IActionResult> DeleteExpense([FromRoute] Guid expenseId)
        {
            if (await _expenseService.Delete(expenseId))
                return Ok("Expense is deleted!");
            else
                throw new ApiException()
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Title = "Can't delete expense",
                    Detail = "Error occured while deleting expense on server"
                };
        }
        [HttpPut("{expenseId:Guid}")]
        public async Task<IActionResult> AddItemsToExpense([FromRoute] Guid expenseId, [FromBody] List<ItemIdDTO> itemList)
        {
            if (await _expenseService.AddItems(expenseId, itemList))
                return Ok("Items successfully added to expense!");
            else
                throw new ApiException()
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Title = "Can't add items",
                    Detail = "Error occured while adding items to expense"
                };
        }
        [HttpGet("{expenseId:Guid}/items")]
        public async Task<IActionResult> GetExpenseItems([FromRoute] Guid expenseId, CancellationToken token)
        {
            if (expenseId == Guid.Empty)
                throw new ApiException()
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Title = "Guid empty",
                    Detail = "No expense guid"
                };

            var expenseItems = await _expenseService.GetExpenseItems(expenseId, token);
            if (expenseItems == null)
                throw new ApiException()
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Title = "Can't find items",
                    Detail = "There is no items for this expense"
                };
            else
                return Ok(expenseItems);
        }
    }
}