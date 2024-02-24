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

namespace EST.BL.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly ExpensesContext _context;
        public ExpenseService(ExpensesContext context)
        {
            _context = context;
        }
        public async Task<List<Expense>> GetAll()
        {
            return await _context.Expenses.ToListAsync();
        }

        public async Task<Expense> GetById(Guid id)
        {
            return await _context.Expenses.Where(e => e.Id == id).FirstOrDefaultAsync();
        }
        public async Task<Expense> Create(ExpenseDTO expenseDto)
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
                return await _context.Expenses.Where(e => e.Date == expense.Date).FirstOrDefaultAsync();
            else
                return null;
        }
        public async Task<bool> Update(ExpenseDTO expenseDto)
        {
            var expense = new Expense()
            {
                Price = expenseDto.Price,
                Date = expenseDto.Date,
                CategoryId = expenseDto.CategoryId,
            };
            _context.Expenses.Update(expense);
            return await SaveAsync();
        }
        public async Task<bool> Delete(Guid id)
        {
            var expense = await GetById(id);
            if (expense == null)
                return false;
            _context.Expenses.Remove(expense);
            return await SaveAsync();
        }
        public async Task<bool> AddItems(Guid id, List<ItemIdDTO> itemList)
        {
            List<ItemExpense> list = itemList.Select(i => new ItemExpense()
            {
                ExpenseId = id,
                ItemId = i.Id
            }).ToList();

            await _context.ItemExpenses.AddRangeAsync(list);
            return await SaveAsync();
        }
        public async Task<List<ExpenseItemsDTO>> GetExpenseItems(Guid id)
        {
            return await _context.ItemExpenses
                .Include(it => it.ItemId)
                .Where(ie => ie.ExpenseId == id)
                .Select(i => 
                new ExpenseItemsDTO()
                {
                    Name = i.Item.Name,
                    IsPublic = i.Item.IsPublic,
                }).ToListAsync();
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
