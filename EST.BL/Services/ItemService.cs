using EST.BL.Interfaces;
using EST.DAL.DataAccess.EF;
using EST.DAL.Models;
using EST.Domain.DTOs;
using Microsoft.EntityFrameworkCore;
using EST.Domain.Helpers;
using EST.Domain.Pagination;
using Microsoft.AspNetCore.Http;

namespace EST.BL.Services
{
    public class ItemService : IItemService
    {
        private readonly ExpensesContext _context;
        public ItemService(ExpensesContext context)
        {
            _context = context;
        }

        public async Task<ItemDTO> GetById(Guid itemId, CancellationToken token)
        {
            var item = await _context.Items
                .Where(i => i.Id == itemId)
                .Include(i => i.Reviews)
                .FirstOrDefaultAsync(token);
            if (item != null)
                return new ItemDTO()
                {
                    Id = item.Id,
                    Name = item.Name,
                    Price = item.Price,
                    IsPublic = item.IsPublic,
                    Value = item.Reviews
                        .Where(r => r.ItemId == item.Id)
                        .Average(t => t.Value)
                };
            else
            {
                throw new ApiException()
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Title = "Item not found",
                    Detail = "Server didn't find item on database"
                };
            }
        }
        public async Task<PagedResponse<List<ItemDTO>>> GetItems(PaginationFilter filter, string userId, CancellationToken token)
        {
            var query = await QueryBuilder(filter, userId);
            
            var totalRecords = await query.CountAsync(token);
            var totalPages = (int)Math.Ceiling(totalRecords / (double)filter.PageSize);

            var result = await GetItems(query, token);
            
            return new PagedResponse<List<ItemDTO>>(result, filter.PageNumber, filter.PageSize, totalRecords, totalPages);
        }
        public async Task<PagedResponse<List<ItemDTO>>> GetItemsForAdminToReview(PaginationFilter filter, string userId, CancellationToken token)//not used
        {
            Guid userGuid;
            try
            {
                userGuid = Guid.Parse(userId);
            }
            catch (Exception e)
            {
                throw new ApiException()
                {
                    StatusCode = StatusCodes.Status422UnprocessableEntity,
                    Title = "Can't parse guid",
                    Detail = "Can't parse guid. Something went wrong while getting Admin guid"
                };
            }
            
            var user = await _context.Users.AnyAsync(u => u.Id == userGuid && u.RoleName == "Admin", token);
            if (!user)
            {
                throw new ApiException()
                {
                    StatusCode = StatusCodes.Status403Forbidden,
                    Title = "Not admin role",
                    Detail = "Forbidden. User's role is not admin"
                };
            }
            var query = await QueryBuilder(filter, "");
            
            var totalRecords = await _context.Items.CountAsync(token);
            var totalPages = (int)Math.Ceiling(totalRecords / (double)filter.PageSize);

            var result = await GetItems(query, token);
            
            return new PagedResponse<List<ItemDTO>>(result, filter.PageNumber, filter.PageSize, totalRecords, totalPages);
        }
        private Task<IQueryable<Item>> QueryBuilder(PaginationFilter filter, string userId)
        {
            IQueryable<Item> query = _context.Items
                .Where(i => !i.IsDeleted);
            
            filter.PageNumber = filter.PageNumber < 0 ? 0 : filter.PageNumber;
            filter.PageSize = filter.PageSize > 5 ? 5 : filter.PageSize;

            if (userId is not "")
            {
                Guid userGuid;
                try
                {
                    userGuid = Guid.Parse(userId);
                    query = filter.TypeItemsVisibility switch
                    {
                        "user" => query.Where(i => i.UserId == userGuid),
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
            
            query = filter.SortColumn switch//I have seen this logic several times in different places. it can be extracted into separate service
            {
                "Price" when filter.SortDirection == "asc" =>
                    query.OrderBy(t => t.Price),
                "Price" => query.OrderByDescending(t => t.Price),
                "Name" when filter.SortDirection == "asc" =>
                    query.OrderBy(t => t.Name),
                "Name" => query.OrderByDescending(t => t.Price),
                _ => query
            };
            
            query = query
                .Skip(filter.PageNumber)
                .Take(filter.PageSize);

            return Task.FromResult(query);
        }
        private static async Task<List<ItemDTO>> GetItems(IQueryable<Item> query, CancellationToken token)
        {
            return await query
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
        }
        public async Task<bool> Create(Guid userId, CreateItemDTO itemDto)
        {
            var item = new Item()
            {
                Name = itemDto.Name,
                Price = itemDto.Price,
                IsPublic = false,//false is default value
                IsDeleted = false,
                UserId = userId
            };
            await _context.Items.AddAsync(item);
            return await SaveAsync();
        }
        public async Task<bool> Update(Guid userId, UpdateItemDTO itemDto, Guid itemId)
        {
            var item = await _context.Items.Where(i => i.Id == itemId && i.UserId == userId).FirstOrDefaultAsync();
            if (item == null)
                throw new ApiException()
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Title = "Not found",
                    Detail = "Can't find item on server"
                };
            item.Name = itemDto.Name;
            item.Price = itemDto.Price;
            
            _context.Items.Update(item);
            return await SaveAsync();
        }
        public async Task<bool> UpdateToPublic(Guid adminId, ItemDTO itemDto)
        {
            if (!await _context.Users.Where(i => i.Id == adminId).AnyAsync())
                return false;

            var item = await _context.Items.Where(i => i.Name == itemDto.Name).FirstOrDefaultAsync();
            if (item == null)
                throw new ApiException()
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Title = "Not found",
                    Detail = "Can't find item on server"
                };
            item.Name = itemDto.Name;
            item.Price = itemDto.Price;
            item.IsPublic = itemDto.IsPublic;

            _context.Items.Update(item);
            return await SaveAsync();
        }
        public async Task<bool> SoftDelete(Guid id)
        {
            var item = await _context.Items.Where(i => i.Id == id).FirstOrDefaultAsync();
            if (item == null)
                return false;
            item.IsDeleted = true;
            _context.Items.Update(item);
            return await SaveAsync();
        }
        public async Task<bool> Exist(Guid id)
        {
            return await _context.Items.Where(i => i.Id == id).AnyAsync();
        }
        public async Task<bool> Exist(string name)
        {
            return await _context.Items.Where(i => i.Name == name).AnyAsync();
        }
        public async Task<bool> SaveAsync()
        {
            var saved = await _context.SaveChangesAsync();
            return saved > 0 ? true : false;
        }
    }
}