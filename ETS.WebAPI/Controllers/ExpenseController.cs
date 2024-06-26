﻿using System.Security.Claims;
using EST.BL.Interfaces;
using EST.Domain.DTOs;
using EST.Domain.Helpers;
using EST.Domain.Pagination;
using Microsoft.AspNetCore.Authorization;
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
            [FromQuery] PaginationFilter filterRequest, 
            CancellationToken token)
        {
            var expenses = await _expenseService.GetAll(filterRequest, token);
            return Ok(expenses);
        }

        [HttpGet("users")]
        [Authorize]
        public async Task<IActionResult> GetUserExpensesPagination(
            [FromQuery] PaginationFilter filterRequest,
            CancellationToken token)
        {
            var user = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var expenses = await _expenseService.GetAllUserExpenses(filterRequest,user, token);
            return Ok(expenses);
        }
        [HttpGet("{expenseId:Guid}")]
        public async Task<IActionResult> GetExpensesById(
            [FromQuery] Guid expenseId, CancellationToken token)
        {
            var expense = await _expenseService.GetById(expenseId, token);
            return Ok(expense);
        }
        [HttpPost]
        public async Task<IActionResult> CreateExpense(
            ExpenseCreateDTO expense, CancellationToken token)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            Guid userParseId;
            try
            {
                userParseId = Guid.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            }
            catch (Exception)
            {
                throw new ApiException()
                {
                    StatusCode = StatusCodes.Status422UnprocessableEntity,
                    Title = "Something wrong with user Guid",
                    Detail = "Error occured while parsing guid from user claims"
                };
            }

            var createdExpense = await _expenseService.Create(expense, userParseId, token);
            return Ok(createdExpense);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateExpense(
            [FromBody] ExpenseUpdateDTO expense, CancellationToken token)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            
            Guid userParseId;
            try
            {
                userParseId = Guid.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            }
            catch (Exception)
            {
                throw new ApiException()
                {
                    StatusCode = StatusCodes.Status422UnprocessableEntity,
                    Title = "Something wrong with user Guid",
                    Detail = "Error occured while parsing guid from user claims"
                };
            }

            var result = await _expenseService.Update(expense, userParseId, token);
            return Ok(result);
        }
        [HttpDelete("{expenseId:Guid}")]
        public async Task<IActionResult> DeleteExpense(
            [FromRoute] Guid expenseId, CancellationToken token)
        {
            if (await _expenseService.Delete(expenseId, token))
                return Ok("Expense is deleted!");
            throw new ApiException()
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Title = "Can't delete expense",
                Detail = "Error occured while deleting expense on server"
            };
        }

        [Authorize]
        [HttpDelete("expenses")]
        public async Task<IActionResult> DeleteExpenses(
            [FromBody] List<ExpenseIdsDTO> toDelete, CancellationToken token)
        {
            if (await _expenseService.DeleteExpenses(toDelete, token))
                return Ok("Expense is deleted!");
            throw new ApiException()
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Title = "Can't delete expense",
                Detail = "Error occured while deleting expense on server"
            };
        }
        [Authorize]
        [HttpPost("add-items")]
        public async Task<IActionResult> AddItemsToExpense(
            [FromBody] AddItemsToExpenseDTO itemList, CancellationToken token)
        {
            Guid userParseId;
            try
            {
                userParseId = Guid.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            }
            catch (Exception)
            {
                throw new ApiException()
                {
                    StatusCode = StatusCodes.Status422UnprocessableEntity,
                    Title = "Something wrong with user Guid",
                    Detail = "Error occured while parsing guid from user claims"
                };
            }
            if (await _expenseService.AddItems(userParseId, itemList, token))
                return Ok(new 
                {
                    message = "Items successfully added to expense!"
                });
            throw new ApiException()
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Title = "Can't add items",
                Detail = "Error occured while adding items to expense"
            };
        }

        [HttpPut("items")]
        public async Task<IActionResult> UpdateItemsToExpense(
            [FromBody] AddItemsToExpenseDTO updateDto, CancellationToken token)
        {
            Guid userParseId;
            try
            {
                userParseId = Guid.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            }
            catch (Exception)
            {
                throw new ApiException()
                {
                    StatusCode = StatusCodes.Status422UnprocessableEntity,
                    Title = "Something wrong with user Guid",
                    Detail = "Error occured while parsing guid from user claims"
                };
            }

            var result = await _expenseService.UpdateItemsToExpense(updateDto, userParseId, token);
            if (result)
                return Ok();
            throw new ApiException()
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Title = "Can't update",
                Detail = "Error occured while updating item expense"
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
            return Ok(expenseItems);
        }
        
        [Authorize]
        [HttpGet("expenses-by-category")]
        public async Task<IActionResult> GetExpensesByCategory(CancellationToken token)
        {
            Guid userParseId;
            try
            {
                userParseId = Guid.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            }
            catch (Exception)
            {
                throw new ApiException()
                {
                    StatusCode = StatusCodes.Status422UnprocessableEntity,
                    Title = "Something wrong with user Guid",
                    Detail = "Error occured while parsing guid from user claims"
                };
            }
            
            var result = await _expenseService.GetExpensesCountByCategory(userParseId, token);
            if(result.Count == 0 || result == null)
                throw new ApiException()
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Title = "No expenses by category",
                    Detail = "There is no such expenses by categories. Create them first"
                };
            return Ok(result);
        }

        [Authorize]
        [HttpGet("last-five")]
        public async Task<IActionResult> GetLastTodayFiveExpenses(CancellationToken token)
        {
            Guid userParseId;
            try
            {
                userParseId = Guid.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            }
            catch (Exception)
            {
                throw new ApiException()
                {
                    StatusCode = StatusCodes.Status422UnprocessableEntity,
                    Title = "Something wrong with user Guid",
                    Detail = "Error occured while parsing guid from user claims"
                };
            }

            var result = await _expenseService.GetLastFiveExpenses(userParseId, token);
            if(result.Count == 0 || result == null)
                throw new ApiException()
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Title = "No last expenses from today",
                    Detail = "There is no such expenses created today. Create them first"
                };
            return Ok(result);
        }
        
        [Authorize]
        [HttpGet("monthly-overview")]
        public async Task<IActionResult> GetMonthlyOverview(CancellationToken token)
        {
            Guid userParseId;
            try
            {
                userParseId = Guid.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            }
            catch (Exception)
            {
                throw new ApiException()
                {
                    StatusCode = StatusCodes.Status422UnprocessableEntity,
                    Title = "Something wrong with user Guid",
                    Detail = "Error occured while parsing guid from user claims"
                };
            }
            var result = await _expenseService.GetMonthlyOverview(userParseId, token);
            if (result == null)
                throw new ApiException()
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Title = "No expenses for monthly overview",
                    Detail = "No expenses to calculate monthly overview"
                };
            return Ok(result);
        }

        [Authorize]
        [HttpGet("average-by-category")]
        public async Task<IActionResult> GetAverageMoneySpentInMonthByCategory(CancellationToken token)
        {
            Guid userParseId;
            try
            {
                userParseId = Guid.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            }
            catch (Exception)
            {
                throw new ApiException()
                {
                    StatusCode = StatusCodes.Status422UnprocessableEntity,
                    Title = "Something wrong with user Guid",
                    Detail = "Error occured while parsing guid from user claims"
                };
            }

            var result = await _expenseService.GetAverageMoneySpentInMonthByCategory(userParseId, token);
            return Ok(result);
        }
        [Authorize]
        [HttpGet("count-items")]
        public async Task<IActionResult> GetCountItemsBoughtInCategory(CancellationToken token)
        {
            Guid userParseId;
            try
            {
                userParseId = Guid.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            }
            catch (Exception)
            {
                throw new ApiException()
                {
                    StatusCode = StatusCodes.Status422UnprocessableEntity,
                    Title = "Something wrong with user Guid",
                    Detail = "Error occured while parsing guid from user claims"
                };
            }

            var result = await _expenseService.CountItemsBoughtInCategory(userParseId, token);
            return Ok(result);
        }
        [Authorize]
        [HttpGet("average-in-year")]
        public async Task<IActionResult> GetAverageMoneySpentInMonthByYear(CancellationToken token)
        {
            Guid userParseId;
            try
            {
                userParseId = Guid.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            }
            catch (Exception)
            {
                throw new ApiException()
                {
                    StatusCode = StatusCodes.Status422UnprocessableEntity,
                    Title = "Something wrong with user Guid",
                    Detail = "Error occured while parsing guid from user claims"
                };
            }

            var result = await _expenseService.GetAverageMoneySpentInMonthByYear(userParseId, token);
            return Ok(result);
        }
    }
}