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
using Microsoft.AspNetCore.Mvc;

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
            
            filter.PageNumber = filter.PageNumber < 0 ? 0 : filter.PageNumber;
            filter.PageSize = filter.PageSize > 5 ? 5 : filter.PageSize;
            
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
                .Skip(filter.PageNumber)
                .Take(filter.PageSize);

            var totalRecords = await _context.Expenses.CountAsync();
            var totalPages = (int)Math.Ceiling(totalRecords / (double)filter.PageSize);
            
            var result = await query
                .AsSplitQuery()
                .Include(e => e.ItemExpenses)
                .ThenInclude(ie => ie.Item)
                .ThenInclude(ir => ir.Reviews)
                .Include(c => c.Category)
                .Include(l => l.ExpenseLocations)
                .ThenInclude(el => el.Location)
                .Include(el => el.ExpenseLocations)
                .ThenInclude(l => l.Location)
                .Select(u => new PaginationExpenseItemsDTO()
                {
                    Id = u.Id,
                    Price = u.Price,
                    Date = u.Date,
                    CategoryName = u.Category.Name,
                    Location = u.ExpenseLocations.Select(x => new LocationDTO()
                    {
                        Name = x.Location.Name,
                        Latitude = x.Location.Latitude,
                        Longitude = x.Location.Longitude,
                        Address = x.Location.Address,
                        Save = x.Location.Save
                    }).FirstOrDefault()!,
                    Items = u.ItemExpenses.Select(x => new ItemExpenseDTO()
                    {
                        Name = x.Item.Name,
                        Price = x.Item.Price,
                        Quantity = x.Quantity,
                        Review = x.Item.Reviews
                            .Where(r => r.ItemId == x.Item.Id)
                            .Average(t => t.Value)
                    }).ToList()
                })
                .ToListAsync(token);
                
            return new PagedResponse<List<PaginationExpenseItemsDTO>>(result, filter.PageNumber, filter.PageSize, totalRecords, totalPages);
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

            filter.PageNumber = filter.PageNumber < 0 ? 0 : filter.PageNumber;
            filter.PageSize = filter.PageSize > 5 ? 5 : filter.PageSize;
            
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
                .Skip(filter.PageNumber)
                .Take(filter.PageSize);

            var totalRecords = await _context.Expenses.Where(u => u.UserId == userId).CountAsync();
            var totalPages = (int)Math.Ceiling(totalRecords / (double)filter.PageSize);
            
            var result = await query
                .Where(u => u.UserId == userId)
                .Include(e => e.ItemExpenses)
                .ThenInclude(ie => ie.Item)
                .ThenInclude(ir => ir.Reviews)
                .Include(c => c.Category)
                .Include(l => l.ExpenseLocations)
                .ThenInclude(el => el.Location)
                .Include(el => el.ExpenseLocations)
                .ThenInclude(l => l.Location)
                .Select(u => new PaginationExpenseItemsDTO()
                {
                    Id = u.Id,
                    Price = u.Price,
                    Date = u.Date,
                    CategoryName = u.Category.Name,
                    Location = u.ExpenseLocations.Select(x => new LocationDTO()
                    {
                        Name = x.Location.Name,
                        Latitude = x.Location.Latitude,
                        Longitude = x.Location.Longitude,
                        Address = x.Location.Address,
                        Save = x.Location.Save
                    }).FirstOrDefault(),
                    Items = u.ItemExpenses.Select(x => new ItemExpenseDTO()
                    {
                        Name = x.Item.Name,
                        Price = x.Item.Price,
                        Quantity = x.Quantity,
                        Review = x.Item.Reviews
                            .Where(r => r.ItemId == x.Item.Id)
                            .Average(t => t.Value)
                    }).ToList()
                })
                .ToListAsync(token);
            
            return new PagedResponse<List<PaginationExpenseItemsDTO>>(result, filter.PageNumber, filter.PageSize, totalRecords, totalPages);
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
        public async Task<ExpenseDTO> Create(ExpenseCreateDTO expenseDto, Guid userId, CancellationToken token)
        {
            var expense = new Expense()
            {
                Price = expenseDto.Price,
                Date = DateTime.Now,
                CategoryId = expenseDto.CategoryId,
                UserId = userId
            };
            await _context.Expenses.AddAsync(expense);
            if (await _context.SaveChangesAsync() > 0)
            {
                var expenseGet = await _context.Expenses
                    .Where(e => e.Date == expense.Date)
                    .FirstOrDefaultAsync(token);
                return new ExpenseDTO()
                {
                    Id = expenseGet.Id,
                    Price = expenseGet.Price,
                    Date = expenseGet.Date
                };
            }
            else
                return null;
        }
        public async Task<ExpenseDTO> Update(ExpenseUpdateDTO expenseDto, Guid userId)
        {
            var expense = await _context.Expenses
                .Where(e => e.Id == expenseDto.Id)
                .FirstOrDefaultAsync();
            if (expense == null)
                throw new Exception("No expense found to update");
            
            expense.Price = expenseDto.Price;
            expense.Date = expenseDto.Date;
            expense.CategoryId = expenseDto.CategoryId;
            expense.UserId = userId;
            
            _context.Expenses.Update(expense);
            var isUpdated = await _context.SaveChangesAsync() > 0;
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
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> AddItems(Guid userId, AddItemsToExpenseDTO itemList)
        {
            if (itemList.ExpenseId == Guid.Empty)
                throw new ApiException()
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Title = "Guid empty",
                    Detail = "Expense guid is empty"
                };

            if (itemList == null || itemList.Items.Count == 0)
                throw new ApiException()
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Title = "No items",
                    Detail = "No items to add on server"
                };

            var checkForPrivateItems = await CheckForPrivateItems(userId, itemList.Items);
            if(!checkForPrivateItems)
                throw new ApiException()
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Title = "private item",
                    Detail = "not user's private item"
                };
            
            var list = itemList.Items.Select(i => new ItemExpense()
            {
                ExpenseId = itemList.ExpenseId,
                ItemId = i.Id,
                Quantity = i.Quantity
            }).ToList();
          
            _context.ItemExpenses.AddRange(list);

            var result = await _context.SaveChangesAsync() > 0;
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
                    Price = i.Item.Price,
                    IsPublic = i.Item.IsPublic,
                    Quantity = i.Quantity
                }).ToListAsync(token);
        }

        public async Task<List<CountExpensesByCategoryDTO>> GetExpensesCountByCategory(Guid userId)
        {
            var categories = await _context.Categories.ToListAsync();
            return await _context.Expenses
                .Where(e => e.UserId == userId)
                .Include(c => c.Category)
                .GroupBy(cn => cn.Category.Name)
                .Select(ce => new CountExpensesByCategoryDTO()
                {
                    Name = ce.Key,
                    Count = ce.Count()
                }).ToListAsync();
        }

        public async Task<List<LastFiveExpensesDTO>> GetLastFiveExpenses(Guid userId)
        {
            var todayStart = DateTime.Today;
            var todayEnd = DateTime.Today.AddDays(1);
            return await _context.Expenses
                .Where(e => e.UserId == userId && e.Date > todayStart && e.Date < todayEnd)
                .Include(c => c.Category)
                .OrderByDescending(d => d.Date)
                .Take(5)
                .Select(cn => new LastFiveExpensesDTO()
                {
                    Price = cn.Price,
                    Date = cn.Date,
                    Category = cn.Category.Name
                })
                .ToListAsync();
        }

        public async Task<MonthlyOverviewDTO> GetMonthlyOverview(Guid userId, CancellationToken token)
        {
            var now = DateTime.UtcNow;
            var thisMonthStartDate = new DateTime(now.Year, now.Month, 1);
            var thisMonthEndDate = thisMonthStartDate.AddMonths(1).AddSeconds(-1);

            var previousMonthStartDate = new DateTime(now.Year, now.Month - 1, 1);
            var previousMonthEndDate = previousMonthStartDate.AddMonths(1).AddDays(-1);

            var todayStart = DateTime.Today;
            var todayEnd = DateTime.Today.AddDays(1);

            var statistic = await _context.Users
                .Select(t => new
                {
                    t.Id,
                    ResultThisMonth = t.Expenses
                        .Where(u => u.Date > thisMonthStartDate && u.Date < thisMonthEndDate)
                        .Average(u => (double?)u.Price),
                    ResultPreviousMonth = t.Expenses
                        .Where(u => u.Date > previousMonthStartDate && u.Date < previousMonthEndDate)
                        .Average(u => (double?)u.Price),
                    ResultDaily = t.Expenses
                        .Where(u => u.Date > todayStart && u.Date < todayEnd)
                        .Average(u => (double?)u.Price)
                    
                })
                .FirstOrDefaultAsync(t => t.Id == userId, token);
            
            return new MonthlyOverviewDTO()
            {
                ThisMonth = statistic.ResultThisMonth ?? 0, 
                PreviousMonth = statistic.ResultPreviousMonth ?? 0,
                Daily = statistic.ResultDaily ?? 0
            };
        }
        public async Task<bool> Exist(Guid id)
        {
            return await _context.Expenses.Where(i => i.Id == id).AnyAsync();
        }
    }
}
