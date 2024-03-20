using EST.DAL.Models;
using EST.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EST.BL.Interfaces
{
    public interface IItemService
    {
        //TODO: delete test get methods
        Task<Item> GetById(Guid id, CancellationToken token);
        Task<ItemDTO> GetByName(string name, CancellationToken token);
        Task<List<ItemDTO>> GetPublicItems(CancellationToken token);
        //TODO: Check if i need private user items. Visually no diff for user on frontend
        Task<List<ItemDTO>> GetPrivateUserItems(Guid userId, CancellationToken token);
        Task<List<ItemDTO>> GetAllUserItems(Guid userId, CancellationToken token);
        Task<List<ItemDTO>> GetItemsForAdminToReview(Guid userId, CancellationToken token);
        Task<bool> Create(Guid userId, string itemName);
        Task<bool> Update(Guid userId, string itemName);
        Task<bool> UpdateToPublic(Guid adminId, ItemDTO itemDto);
        Task<bool> SoftDelete(Guid id);
        Task<bool> Exist(Guid id);
        Task<bool> Exist(string name);
        Task<bool> SaveAsync();
    }
}
