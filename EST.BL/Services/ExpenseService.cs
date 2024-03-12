using EST.BL.Interfaces;
using EST.DAL.DataAccess.EF;
using EST.DAL.Models;
using EST.Domain.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EST.Domain.Helpers;
using Microsoft.AspNetCore.Http;

namespace EST.BL.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly ExpensesContext _context;
        public ExpenseService(ExpensesContext context)
        {
            _context = context;
        }
        public async Task<List<Expense>> GetAll(CancellationToken token)
        {
            return await _context.Expenses.ToListAsync(token);
        }

        public async Task<Expense> GetById(Guid id, CancellationToken token)
        {
            return await _context.Expenses.Where(e => e.Id == id).FirstOrDefaultAsync(token);
        }
        public async Task<ExpenseDTO> Create(ExpenseCreateDTO expenseDto, CancellationToken token)
        {
            var expense = new Expense()
            {
                Price = expenseDto.Price,
                Date = expenseDto.Date,
                CategoryId = expenseDto.CategoryId,
                UserId = expenseDto.UserId
            };
            await _context.Expenses.AddAsync(expense);
            if (await SaveAsync())
            {
                var expenseGet = await _context.Expenses.Where(e => e.Date == expense.Date).FirstOrDefaultAsync(token);
                return new ExpenseDTO()
                {
                    Price = expenseGet.Price,
                    Date = expenseDto.Date
                };
            }
            else
                return null;
        }
        public async Task<ExpenseDTO> Update(ExpenseUpdateDTO expenseDto, Guid userId)
        {
            var expense = await _context.Expenses.Where(e => e.Id == expenseDto.Id).FirstOrDefaultAsync();
            if (expense == null)
                throw new Exception("No expense found to update");
            
            expense.Price = expenseDto.Price;
            expense.Date = expenseDto.Date;
            expense.CategoryId = expenseDto.CategoryId;
            expense.UserId = userId;
            _context.Expenses.Update(expense);
            var isUpdated = await SaveAsync();
            if (isUpdated)
                return new ExpenseDTO()
                {
                    Date = expense.Date,
                    Price = expense.Price
                };
            else
                return null;
        }
        public async Task<bool> Delete(Guid id)
        {
            if (id == Guid.Empty)
                throw new ApiException()
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Title = "Guid empty",
                    Detail = "Expense guid is empty"
                };

            var isExists = await Exist(id);
            if (!isExists)
                throw new ApiException()
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Title = "No expense found",
                    Detail = "Expense doesn't existw"
                };
                    
            var expense = await _context.Expenses.Where(u => u.Id == id).FirstOrDefaultAsync();
            if (expense == null)
                return false;
            _context.Expenses.Remove(expense);
            return await SaveAsync();
        }
        public async Task<bool> AddItems(Guid id, List<ItemIdDTO> itemList)
        {
            if (id == Guid.Empty)
                throw new ApiException()
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Title = "Guid empty",
                    Detail = "Expense guid is empty"
                };

            if (itemList == null || itemList.Count == 0)
                throw new ApiException()
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Title = "No items",
                    Detail = "No items to add on server"
                };
            
            var list = itemList.Select(i => new ItemExpense()
            {
                ExpenseId = id,
                ItemId = i.Id
            }).ToList();

            await _context.ItemExpenses.AddRangeAsync(list);
            return await SaveAsync();
        }
        public async Task<List<ExpenseItemsDTO>> GetExpenseItems(Guid id, CancellationToken token)
        {
            return await _context.ItemExpenses
                .Include(it => it.Item)
                .Where(ie => ie.ExpenseId == id)
                .Select(i => 
                new ExpenseItemsDTO()
                {
                    Name = i.Item.Name,
                    IsPublic = i.Item.IsPublic,
                }).ToListAsync(token);
        }
        public async Task<bool> Exist(Guid id)
        {
            return await _context.Expenses.Where(i => i.Id == id).AnyAsync();
        }
        public async Task<bool> SaveAsync()
        {
            var saved = await _context.SaveChangesAsync();
            return saved > 0 ? true : false;
        }
    }
}
