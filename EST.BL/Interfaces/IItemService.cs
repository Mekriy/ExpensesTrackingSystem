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
        Task<Item> GetById(Guid id, CancellationToken token);
        Task<ItemDTO> GetByName(string name, CancellationToken token);
        Task<List<ItemDTO>> GetItemsToReview(CancellationToken token);
        Task<bool> Create(ItemDTO itemDto);
        Task<bool> Update(ItemDTO itemDto);
        Task<bool> UpdateToPublic(Guid adminId, ItemDTO itemDto);
        Task<bool> Delete(Guid id);
        Task<bool> Exist(Guid id);
        Task<bool> Exist(string name);
        Task<bool> SaveAsync();
        Task<List<Item>> GetAll(CancellationToken token);
    }
}
