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
        public async Task<Category> GetById(Guid id, CancellationToken token)
        {
            return await _context.Categories.Where(i => i.Id == id).FirstOrDefaultAsync(token);
        }
        public async Task<bool> Create(CreateCategoryDTO categoryDTO)
        {
            var user = await _context.Users.Where(i => i.Id == categoryDTO.userId).FirstOrDefaultAsync();
            if (user == null)
                return false;

            Category category;
            if (user.RoleName == "Admin")
            {
                category = new Category()
                {
                    Name = categoryDTO.Name,
                    IsPublic = true,
                    IsDeleted = false,
                    UserId = categoryDTO.userId,
                };
            }
            else
            {
                category = new Category()
                {
                    Name = categoryDTO.Name,
                    IsPublic = false,
                    IsDeleted = false,
                    UserId = categoryDTO.userId,
                };
            }

            await _context.Categories.AddAsync(category);
            return await SaveAsync();
        }
        public async Task<bool> Update(UpdateCategoryDTO categoryDTO)
        {
            var category = await _context.Categories.Where(c => c.Name == categoryDTO.OldName).FirstOrDefaultAsync();
            category.Name = categoryDTO.NewName;
            
            _context.Categories.Update(category);
            return await SaveAsync();
        }
        public async Task<bool> Delete(Guid id)
        {
            var category = await _context.Categories.Where(u => u.Id == id).FirstOrDefaultAsync();
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

        public async Task<List<CategoryDTO>> GetPublic()
        {
            var result = await _context.Categories
                .Where(c => c.IsPublic)
                .Select(c => new CategoryDTO()
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync();
            return result;
        }
    }
}
