using EST.BL.Interfaces;
using EST.DAL.DataAccess.EF;
using EST.DAL.Models;
using EST.Domain.DTOs;
using EST.Domain.Helpers;
using EST.Domain.Pagination;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EST.BL.Services;

public class AdminService : IAdminService
{
    private readonly ExpensesContext _context;

    public AdminService(ExpensesContext context)
    {
        _context = context;
    }
    
    public async Task<PagedResponse<List<ItemDTO>>> GetItemsForAdminToReview(PaginationFilter filter, string userId, CancellationToken token)
    {
        IQueryable<Item> query = _context.Items;
            
        filter.PageNumber = filter.PageNumber < 0 ? 0 : filter.PageNumber;
        filter.PageSize = filter.PageSize > 5 ? 5 : filter.PageSize;

        if (userId is not "")
        {
            Guid userGuid;
            try
            {
                userGuid = Guid.Parse(userId);
                var checkUser = await _context.Users.Where(u => u.Id == userGuid).AnyAsync(token);
                if (!checkUser)
                {
                    throw new Exception("no user");
                }
                query = filter.TypeItemsVisibility switch
                {
                    "private" => query.Where(i => !i.IsPublic),
                    "all" => query.Where(i => i.IsPublic),
                    _ => query.Where(i => i.IsPublic)
                };
            }
            catch (Exception e)
            {
                throw new ApiException()
                {
                    StatusCode = StatusCodes.Status422UnprocessableEntity,
                    Title = "Can't parse guid",
                    Detail = "Can't parse guid. Something went wrong while doing item pagination"
                };
            }
        }
            
        query = filter.SortColumn switch
        {
            "Price" when filter.SortDirection == "asc" =>
                query.OrderBy(t => t.Price),
            "Price" => query.OrderByDescending(t => t.Price),
            "Name" when filter.SortDirection == "asc" =>
                query.OrderBy(t => t.Name),
            "Name" => query.OrderByDescending(t => t.Price),
            _ => query
        };
            
        var totalRecords = await query.CountAsync(token);
        var totalPages = (int)Math.Ceiling(totalRecords / (double)filter.PageSize);
        
        query = query
            .Skip(filter.PageNumber)
            .Take(filter.PageSize);

        var result = await query
            .Select(i => new ItemDTO()
            {
                Id = i.Id,
                Name = i.Name,
                Price = i.Price,
                IsPublic = i.IsPublic,
                Value = i.Reviews.Any(r => r.ItemId == i.Id) ? 
                    i.Reviews.Where(r => r.ItemId == i.Id).Average(t => t.Value) : 0,
                Quantity = 1
            }).ToListAsync(token);
            
        return new PagedResponse<List<ItemDTO>>(result, filter.PageNumber, filter.PageSize, totalRecords, totalPages);
    }

    public async Task<List<TodaysExpensesByCategoriesDTO>> GetStatistic(CancellationToken token)
    {
        var now = DateTime.Now;
        var thisDayStartDate = now.Date;
        var thisDayEndDate = thisDayStartDate.AddDays(1);

        return await _context.Expenses
            .Include(c => c.Category)
            .Where(e => e.Date > thisDayStartDate && e.Date < thisDayEndDate && e.Category.IsPublic)
            .GroupBy(c => c.Category.Name)
            .Select(t => new TodaysExpensesByCategoriesDTO()
            {
                CategoryName = t.Key,
                ExpenseCount = t.Count(),
            })
            .ToListAsync(token);
    }

    public async Task<GeneralInfoOfTodayDTO> GetGeneralInfo(CancellationToken token)
    {
        var now = DateTime.Now;
        var thisDayStartDate = now.Date;
        var thisDayEndDate = thisDayStartDate.AddDays(1);

        var countAllExpensesOfToday = await _context.Expenses
            .Where(e => e.Date > thisDayStartDate && e.Date < thisDayEndDate)
            .CountAsync(token);

        var averageAllExpensesOfToday = await _context.Expenses
            .Where(e => e.Date > thisDayStartDate && e.Date < thisDayEndDate)
            .AverageAsync(e => e.Price, token);
        
        var popularCategoryOfToday = await _context.Expenses
            .Include(c => c.Category)
            .Where(e => e.Date >= thisDayStartDate && e.Date < thisDayEndDate && e.Category.IsPublic)
            .GroupBy(c => c.Category.Name)
            .Select(t => new GeneralInfoOfTodayDTO()
            {
                CategoryName = t.Key,
                Count = t.Count(),
            })
            .OrderByDescending(arg => arg.Count)
            .Take(1)
            .FirstAsync(token);
        
        return new GeneralInfoOfTodayDTO()
        {
            CategoryName = popularCategoryOfToday.CategoryName,
            Count = popularCategoryOfToday.Count,
            AllExpensesCount = countAllExpensesOfToday,
            AverageExpensePrice = averageAllExpensesOfToday
        };
    }

    public async Task<bool> ChangeItemsVisibility(ItemDTO itemDto, CancellationToken token)
    {
        var item = await _context.Items
            .Where(i => !i.IsPublic && i.Id == itemDto.Id)
            .FirstOrDefaultAsync(token);
        item.IsPublic = true;
        _context.Update(item);
        return await _context.SaveChangesAsync(token) > 0;
    }

    public async Task<List<UserWithPhotoDTO>> GetUsersByQuery(string user, CancellationToken token)
    {
        var result = await _context.Users
            .Include(p => p.PhotoFile)
            .Where(u => u.FirstName.ToLower().Contains(user))
            .Select(t => new UserWithPhotoDTO()
            {
                Id = t.Id,
                FirstName = t.FirstName,
                LastName = t.LastName,
                Email = t.Email, 
                FileName = t.PhotoFile.FileName,
                RoleName = t.RoleName
            })
            .ToListAsync(token);
        
        return result.Count > 0 ? result : throw new ApiException()
        {
            StatusCode = StatusCodes.Status404NotFound,
            Title = "Not found",
            Detail = "Search term was not find on db"
        };
    }

    public async Task<UsersCreatedInfoDTO> GetUsersCreatedInfo(Guid userId, CancellationToken token)
    {
        var result = await _context.Users
            .Select(t => new
            {
                t.Id,
                items = _context.Items
                    .Where(i => i.UserId == t.Id)
                    .Include(r => r.Reviews)
                    .Select(it => new ItemDTO()
                    {
                        Id = it.Id,
                        Name = it.Name,
                        Price = it.Price,
                        Value = it.Reviews.Any(r => r.ItemId == it.Id) ? 
                            it.Reviews.Where(r => r.ItemId == it.Id).Average(rv => rv.Value) : 0,
                    })
                    .ToList(),
                categories = _context.Categories
                    .Where(c => c.UserId == t.Id)
                    .Select(ct => new CategoryDTO()
                    {
                        Id = ct.Id,
                        Name = ct.Name
                    })
                    .ToList(),
                locations = _context.Locations
                    .Where(l => l.UserId == t.Id && l.Save == true)
                    .Select(lt => new LocationDTO()
                    {
                        Name = lt.Name,
                        Latitude = lt.Latitude,
                        Longitude = lt.Longitude,
                        Address = lt.Address,
                        Save = lt.Save,
                    })
                    .ToList()
            }).FirstOrDefaultAsync(u => u.Id == userId, token);

        return new UsersCreatedInfoDTO()
        {
            Items = result.items,
            Categories = result.categories,
            Locations = result.locations
        };
    }
} 