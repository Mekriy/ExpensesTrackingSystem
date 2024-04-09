﻿using EST.DAL.Models;
using EST.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EST.Domain.Pagination;

namespace EST.BL.Interfaces
{
    public interface IItemService
    {
        Task<ItemDTO> GetById(Guid ItemId, CancellationToken token);
        Task<PagedResponse<List<ItemDTO>>> GetPublicItems(PaginationFilter filter, CancellationToken token);
        //TODO: Check if i need private user items. Visually no diff for user on frontend
        Task<PagedResponse<List<ItemDTO>>> GetPrivateUserItems(PaginationFilter filter, string userId, CancellationToken token);
        Task<PagedResponse<List<ItemDTO>>> GetAllUserItems(PaginationFilter filter, string userId, CancellationToken token);
        Task<PagedResponse<List<ItemDTO>>> GetItemsForAdminToReview(PaginationFilter filter, string userId, CancellationToken token);
        Task<bool> Create(Guid userId, CreateItemDTO item);
        Task<bool> Update(Guid userId, UpdateItemDTO item, Guid itemId);
        Task<bool> UpdateToPublic(Guid adminId, ItemDTO itemDto);
        Task<bool> SoftDelete(Guid id);
        Task<bool> Exist(Guid id);
        Task<bool> Exist(string name);
        Task<bool> SaveAsync();
    }
}
