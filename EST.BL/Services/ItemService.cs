﻿using EST.BL.Interfaces;
using EST.DAL.DataAccess.EF;
using EST.DAL.Models;
using EST.Domain.DTOs;
using Microsoft.EntityFrameworkCore;
using EST.Domain.Helpers;
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
        public async Task<Item> GetById(Guid id, CancellationToken token)
        {
            return await _context.Items.Where(i => i.Id == id).FirstOrDefaultAsync(token);
        }
        public async Task<ItemDTO> GetByName(string name, CancellationToken token)
        {
            var item = await _context.Items.Where(i => i.Name == name).FirstOrDefaultAsync(token);
            var review = await _context.Reviews.Where(r => r.ItemId == item.Id).ToListAsync(token);

            var itemDTO = new ItemDTO()
            {
                Name = item.Name,
                IsPublic = item.IsPublic,
                Value = review.Sum(l => l.Value) / review.Count
            };

            return itemDTO;
        }
        public async Task<List<ItemDTO>> GetPublicItems(CancellationToken token)
        {
            return await _context.Items.Where(i => i.IsPublic == true && i.IsDeleted == false).Select(i => new ItemDTO()
            {
                Name = i.Name,
                IsPublic = i.IsPublic,
            }).ToListAsync(token);
        }

        public async Task<List<ItemDTO>> GetPrivateUserItems(Guid userId, CancellationToken token)
        {
            return await _context.Items.Where(i => i.UserId == userId && i.IsPublic == false && i.IsDeleted == false).Select(i => new ItemDTO()
            {
                Name = i.Name,
                Price = i.Price,
                IsPublic = i.IsPublic,
            }).ToListAsync(token);
        }
        public async Task<List<ItemDTO>> GetAllUserItems(Guid userId, CancellationToken token)
        {
            return await _context.Items.Where(i => i.UserId == userId && i.IsDeleted == false).Select(i => new ItemDTO()
            {
                Name = i.Name,
                Price = i.Price,
                IsPublic = i.IsPublic,
            }).ToListAsync(token);
        }
        public async Task<List<ItemDTO>> GetItemsForAdminToReview(Guid userId, CancellationToken token)
        {
            var user = await _context.Users.Where(u => u.Id == userId).FirstOrDefaultAsync(token);
            if (user.RoleName != "Admin")
                throw new ApiException()
                {
                    StatusCode = StatusCodes.Status403Forbidden,
                    Title = "Not admin role",
                    Detail = "Forbidden. User's role is not admin"
                };
            
            return await _context.Items.Where(i => i.IsPublic == false && i.IsDeleted == false).Select(i => new ItemDTO()
            {
                Name = i.Name,
                Price = i.Price,
                IsPublic = i.IsPublic,
            }).ToListAsync(token);
        }
        public async Task<bool> Create(Guid userId, CreateItemDTO itemDto)
        {
            var item = new Item()
            {
                Name = itemDto.Name,
                Price = itemDto.Price,
                IsPublic = false,
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
            item.Name = item.Name;
            item.Price = item.Price;
            
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
