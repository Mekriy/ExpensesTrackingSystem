using EST.BL.Interfaces;
using EST.DAL.DataAccess.EF;
using EST.DAL.Models;
using EST.Domain.DTOs;
using Microsoft.EntityFrameworkCore;

namespace EST.BL.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ExpensesContext _context;
        public CategoryService(ExpensesContext context)
        {
            _context = context;
        }
        public async Task<Category> GetById(Guid id)
        {
            return await _context.Categories.Where(i => i.Id == id).FirstOrDefaultAsync();
        }
        public async Task<bool> Create(CategoryDTO categoryDTO)
        {
            var category = new Category()
            {
                Name = categoryDTO.Name,
                IsPublic = false,
                IsDeleted = false,
                UserId = categoryDTO.UserId,
            };

            await _context.Categories.AddAsync(category);
            return await SaveAsync();
        }
        public async Task<bool> Update(UpdateCategoryDTO categoryDTO)
        {
            var category = await _context.Categories.Include(u => u.User).Where(c => c.Name == categoryDTO.OldName).FirstOrDefaultAsync();
            category.Name = categoryDTO.NewName;
            
            _context.Categories.Update(category);
            return await SaveAsync();
        }
        public async Task<bool> Delete(Guid id)
        {
            var category = await GetById(id);
            var deletedCategory = new Category()
            {
                Name = category.Name,
                IsDeleted = true,
            };
            _context.Categories.Update(deletedCategory);
            return await SaveAsync();
        }
        public async Task<bool> Exist(Guid id)
        {
            return await _context.Categories.Where(i => i.Id == id).AnyAsync();
        }
        public async Task<bool> Exist(string name)
        {
            return await _context.Categories.Where(i => i.Name == name).AnyAsync();
        }
        public async Task<bool> SaveAsync()
        {
            var saved = await _context.SaveChangesAsync();
            return saved > 0 ? true : false;
        }
    }
}
