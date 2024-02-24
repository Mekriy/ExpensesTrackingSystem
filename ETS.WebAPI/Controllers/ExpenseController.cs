using EST.BL.Interfaces;
using EST.DAL.Models;
using EST.Domain.DTOs;
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
        public async Task<IActionResult> GetAll()
        {
            var expenses = await _expenseService.GetAll();
            if (expenses == null || expenses.Count == 0)
                return BadRequest("There are no expenses!");
            else
                return Ok(expenses);
        }
        [HttpGet("{expenseId:Guid}")]
        public async Task<IActionResult> GetExpensesById([FromQuery] Guid expenseId)
        {
            var expense = await _expenseService.GetById(expenseId);
            if (expense == null)
                return BadRequest("No expense!");
            return Ok(expense);
        }
        [HttpPost]
        public async Task<IActionResult> CreateExpense(ExpenseDTO expense)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            if (expense == null)
                return BadRequest("No expense!");

            if (await _expenseService.Create(expense))
                return Ok("Expense is created");
            else
                return StatusCode(500, "Error occured while creating expense on server");
        }
        [HttpPut]
        public async Task<IActionResult> UpdateExpense(ExpenseDTO expense)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            if (expense == null)
                return BadRequest("No expense");

            if (await _expenseService.Update(expense))
                return Ok("Expense is updated!");
            else
                return StatusCode(500, "Error occured while updating expense on server");
        }
        [HttpDelete("{expenseId:Guid}")]
        public async Task<IActionResult> DeleteExpense([FromQuery] Guid expenseId)
        {
            if (expenseId == Guid.Empty)
                return BadRequest("No guid");

            if (!await _expenseService.Exist(expenseId))
                return BadRequest("Expense doesn't exist");

            if (await _expenseService.Delete(expenseId))
                return Ok("Expense is deleted!");
            else
                return StatusCode(500, "Error occured while deleting expense on server");
        }
    }
}
