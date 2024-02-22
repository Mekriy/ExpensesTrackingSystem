using EST.BL.Interfaces;
using EST.DAL.Models;
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
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllExpenses()
        {
            var expenses = await _expenseService.GetAll();
            if (expenses == null || expenses.Count == 0)
                return BadRequest("There are no expenses!");
            else
                return Ok(expenses);
        }
        [HttpGet("GetByGuid")]
        public async Task<IActionResult> GetExpensesById([FromQuery] Guid expenseId)
        {
            var expense = await _expenseService.GetById(expenseId);
            if (expense == null)
                return BadRequest("No expense!");
            return Ok(expense);
        }
        [HttpPost("Create")]
        public async Task<IActionResult> CreateExpense(Expense expense)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            if (expense == null)
                return BadRequest("No expense!");

            if (await _expenseService.Exist(expense.Id))
                return BadRequest("Expense already exists");

            if (await _expenseService.Create(expense))
                return Ok("Expense is created");
            else
                return StatusCode(500, "Error occured while creating expense on server");
        }
        [HttpPut("Update")]
        public async Task<IActionResult> UpdateExpense(Expense expense)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            if (expense == null)
                return BadRequest("No expense");

            if (!await _expenseService.Exist(expense.Id))
                return BadRequest("Expense doesn't exist");

            if (await _expenseService.Update(expense))
                return Ok("Expense is updated!");
            else
                return StatusCode(500, "Error occured while updating expense on server");
        }
        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteExpense([FromQuery] Guid Id)
        {
            if (Id == Guid.Empty)
                return BadRequest("No guid");

            if (!await _expenseService.Exist(Id))
                return BadRequest("Expense doesn't exist");

            if (await _expenseService.Delete(Id))
                return Ok("Expense is deleted!");
            else
                return StatusCode(500, "Error occured while deleting expense on server");
        }
    }
}
