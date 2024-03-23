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
using EST.Domain.Pagination;
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
        public async Task<PagedResponse<List<PaginationExpenseItemsDTO>>> GetAll(PaginationFilter filter, CancellationToken token)
        {
            IQueryable<Expense> query = _context.Expenses;

            query = filter.SortColumn switch
            {
                "Price" when filter.SortDirection == "asc" =>
                    query.OrderBy(t => t.Price),
                "Price" => query.OrderByDescending(t => t.Price),
                "Date" when filter.SortDirection == "asc" =>
                    query.OrderBy(t => t.Date),
                "Date" => query.OrderByDescending(t => t.Price),
                _ => query
            };

            query = query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize);

            var result = await query
                .Include(e => e.ItemExpenses)
                .ThenInclude(ie => ie.Item)
                .Select(u => new PaginationExpenseItemsDTO()
                {
                    Price = u.Price,
                    Date = u.Date,
                    Name = u.ItemExpenses.Select(ie => ie.Item.Name).ToList(),
                    Quantity = u.ItemExpenses.Select(ie => ie.Quantity).ToList()
                })
                .ToListAsync(token);
                
            return new PagedResponse<List<PaginationExpenseItemsDTO>>(result, filter.PageNumber, filter.PageSize);
        }
        public async Task<PagedResponse<List<PaginationExpenseItemsDTO>>> GetAllUserExpenses(PaginationFilter filter, string user, CancellationToken token)
        {
            Guid userId;
            try
            {
                 userId = Guid.Parse(user);
            }
            catch (Exception e)
            {
                throw new ApiException()
                {
                    StatusCode = StatusCodes.Status422UnprocessableEntity,
                    Title = "Can't parse guid",
                    Detail = "Can't parse guid. Something went wrong"
                };
            }
            
            IQueryable<Expense> query = _context.Expenses;

            query = filter.SortColumn switch
            {
                "Price" when filter.SortDirection == "asc" =>
                    query.OrderBy(t => t.Price),
                "Price" => query.OrderByDescending(t => t.Price),
                "Date" when filter.SortDirection == "asc" =>
                    query.OrderBy(t => t.Date),
                "Date" => query.OrderByDescending(t => t.Price),
                _ => query
            };

            query = query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize);
            
            var result = await query
                .Where(u => u.UserId == userId)
                .Include(e => e.ItemExpenses)
                .ThenInclude(ie => ie.Item)
                .Select(u => new PaginationExpenseItemsDTO()
                {
                    Price = u.Price,
                    Date = u.Date,
                    Name = u.ItemExpenses.Select(ie => ie.Item.Name).ToList(),
                    Quantity = u.ItemExpenses.Select(ie => ie.Quantity).ToList()
                })
                .ToListAsync(token);
            
            return new PagedResponse<List<PaginationExpenseItemsDTO>>(result, filter.PageNumber, filter.PageSize);
        }
        public async Task<ExpenseDTO> GetById(Guid id, CancellationToken token)
        {
            var expense = await _context.Expenses.Where(e => e.Id == id).FirstOrDefaultAsync(token);
            return new ExpenseDTO()
            {
                Date = expense.Date,
                Price = expense.Price
            };
        }
        public async Task<ExpenseDTO> Create(ExpenseCreateDTO expenseDto,Guid userId, CancellationToken token)
        {
            var expense = new Expense()
            {
                Price = expenseDto.Price,
                Date = DateTime.UtcNow,
                CategoryId = expenseDto.CategoryId,
                UserId = userId
            };
            await _context.Expenses.AddAsync(expense);
            if (await SaveAsync())
            {
                var expenseGet = await _context.Expenses.Where(e => e.Date == expense.Date).FirstOrDefaultAsync(token);
                return new ExpenseDTO()
                {
                    Price = expenseGet.Price,
                    Date = expenseGet.Date
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
        public async Task<bool> AddItems(Guid userId, Guid expenseId, List<ItemIdDTO> itemList)
        {
            if (expenseId == Guid.Empty)
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

            var checkForPrivateItems = await CheckForPrivateItems(userId, itemList);
            if(!checkForPrivateItems)
                throw new ApiException()
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Title = "private item",
                    Detail = "not user's private item"
                };
            
            List<ItemExpense> list = itemList.Select(i => new ItemExpense()
            {
                ExpenseId = expenseId,
                ItemId = i.Id,
                Quantity = i.Quantity
            }).ToList();
          
            _context.ItemExpenses.AddRange(list);

            var result = await SaveAsync();
            if (result)
                return true;
            else
                throw new ApiException()
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Title = "Error while saving",
                    Detail = "Error occured while saving item expense on server"
                };
        }

        private async Task<bool> CheckForPrivateItems(Guid userId, List<ItemIdDTO> itemList)
        {
            var items = itemList
                .Select(x => _context.Items.FirstOrDefault(i => i.Id == x.Id))
                .Where(i=> !i.IsPublic)
                .ToList();
            var result = true;
            result = items.All(i => i.UserId == userId);
            
            return result;
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
                    Quantity = i.Quantity
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
