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
    public class ItemService : IItemService
    {
        private readonly ExpensesContext _context;
        public ItemService(ExpensesContext context)
        {
            _context = context;
        }
        public async Task<List<Item>> GetAll()
        {
            return await _context.Items.ToListAsync();
        }
        public async Task<Item> GetById(Guid id)
        {
            return await _context.Items.Where(i => i.Id == id).FirstOrDefaultAsync();
        }
        public async Task<bool> Create(ItemDTO itemDto)
        {
            var item = new Item()
            {
                Name = itemDto.Name,
                IsPublic = false
            };
            await _context.Items.AddAsync(item);
            return await SaveAsync();
        }
        public async Task<bool> Update(ItemDTO itemDto)
        {
            var item = new Item()
            {
                Name = itemDto.Name,
                IsPublic = itemDto.IsPublic
            };
            _context.Items.Update(item);
            return await SaveAsync();
        }
        public async Task<bool> Delete(Guid id)
        {
            var item = await _context.Items.Where(i => i.Id == id).FirstOrDefaultAsync();
            if (item == null)
                return false;
            _context.Items.Remove(item);
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
