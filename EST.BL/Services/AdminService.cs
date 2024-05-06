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
            
        filter.PageNumber = filter.PageNumber < 0 ? 0 : filter.PageNumber;//very strange. why not using uint or validate model using helpers?
        filter.PageSize = filter.PageSize > 5 ? 5 : filter.PageSize;

        if (userId is not "")
        {
            Guid userGuid;
            try
            {
                //why do you check if such user exists? you received a token with admin claim.
                //token can't be invalid and no one can change token body manually, it invalidates signature
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
                    Title = "Can't parse guid",//there is guid.tryparse method. this handing is not OK
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
        
        query = query//no need to assign this here. way better to call it down below
            .Skip(filter.PageNumber)
            .Take(filter.PageSize);

        var result = await query
            .Where(i => !i.IsDeleted)
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
        var now = DateTime.Now;//very bad practice to use DateTime.Now. it depends on your time zone.
                               //it works locally but if you want to deploy the code somewhere it will break everything.
                               //DateTime.UtcNow is your choice
                               //lines 96-97 are not needed. can be shortened
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
        var now = DateTime.Now;//same as above
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
            .Take(1)//you don't need take(1) if you make firstasync
            .FirstAsync(token);
        
        return new GeneralInfoOfTodayDTO()
        {
            CategoryName = popularCategoryOfToday.CategoryName,
            Count = popularCategoryOfToday.Count,
            AllExpensesCount = countAllExpensesOfToday,
            AverageExpensePrice = averageAllExpensesOfToday
        };
    }

    public async Task<bool> ChangeItemsVisibility(ItemToBePublicDTO itemDto, Guid adminId, CancellationToken token)
    {
        var item = await _context.Items
            .Where(i => !i.IsPublic && i.Id == itemDto.Id)
            .FirstOrDefaultAsync(token);
        item.IsPublic = true;
        var review = new Review()
        {
            ItemId = item.Id,
            UserId = adminId,
            Value = itemDto.Review
        };
        _context.Update(item);
        _context.Reviews.Add(review);
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
            .Select(t => new//anonymous object is not OK. also I see a lot of routine mapping which can be extracted or automated
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

    public async Task<List<CategoryDTO>> GetCategories(CancellationToken token)
    {
        return await _context.Categories.Where(c => c.IsPublic && !c.IsDeleted)
            .Select(t => new CategoryDTO()
            {
                Id = t.Id,
                Name = t.Name
            })
            .ToListAsync(token);
    }

    //don't you have an id of category? this operation is very strange. you need additional indexing in case of big number of names
    public async Task<bool> UpdateCategory(UpdateCategoryDTO update, CancellationToken token)
    {
        var category = await _context.Categories.Where(c => c.Name == update.OldName && c.IsPublic)
            .FirstOrDefaultAsync(token);
        category.Name = update.NewName;
            
        _context.Categories.Update(category);
        return await _context.SaveChangesAsync(token) > 0;
    }

    //same as above. you can even unify methods and endpoints
    public async Task<bool> DeleteCategory(string name, CancellationToken token)
    {
        var category = await _context.Categories
            .Where(u => u.Name == name && u.IsPublic)
            .FirstOrDefaultAsync(token);
        if (category.IsDeleted)
        {
            throw new ApiException()
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Title = "Category is already deleted!",
                Detail = "Error occured because category is already deleted"
            };
        }

        category.IsDeleted = true;
        _context.Categories.Update(category);
        return await _context.SaveChangesAsync(token)>0;
    }
} 