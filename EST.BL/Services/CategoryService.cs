using EST.BL.Interfaces;
using EST.DAL.DataAccess.EF;
using EST.DAL.Models;
using EST.Domain.DTOs;
using EST.Domain.Helpers;
using Microsoft.AspNetCore.Http;
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
        public async Task<CategoryDTO> Create(CreateCategoryDTO categoryDTO, Guid userId, CancellationToken token)
        {
            var user = await _context.Users.Where(i => i.Id == userId).FirstOrDefaultAsync(token);
            if (user == null)
                return null;

            Category category;
            if (user.RoleName == "Admin")
            {
                category = new Category()
                {
                    Name = categoryDTO.Name,
                    IsPublic = true,
                    IsDeleted = false,
                    UserId = userId,
                };
            }
            else
            {
                category = new Category()
                {
                    Name = categoryDTO.Name,
                    IsPublic = false,
                    IsDeleted = false,
                    UserId = userId,
                };
            }
            await _context.Categories.AddAsync(category, token);
            if (await _context.SaveChangesAsync(token) > 0)
            {
                var created = await _context.Categories
                    .Where(c => c.Name == category.Name)
                    .FirstAsync(token);
                return new CategoryDTO()
                {
                    Id = created.Id,
                    Name = created.Name
                };
            }
                
            throw new ApiException()
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Title = "Can't create category",
                Detail = "Error occured while creating category on server"
            };
        }
        public async Task<bool> Update(UpdateCategoryDTO categoryDTO, Guid userId, CancellationToken token)
        {
            var category = await _context.Categories
                .Where(c => c.Name == categoryDTO.OldName && c.UserId == userId && !c.IsPublic)
                .FirstOrDefaultAsync(token);
            category.Name = categoryDTO.NewName;
            
            _context.Categories.Update(category);
            return await _context.SaveChangesAsync(token) > 0;
        }
        public async Task<bool> Delete(string name, Guid userId, CancellationToken token)
        {
            var category = await _context.Categories
                .Where(u => u.Name == name && u.UserId == userId && !u.IsPublic)
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
        public async Task<bool> Exist(string name, Guid userId, CancellationToken token)
        {
            return await _context.Categories.Where(i => i.Name == name && i.UserId == userId).AnyAsync(token);
        }
        public async Task<List<CategoryDTO>> GetPublic(CancellationToken token)
        {
            var result = await _context.Categories
                .Where(c => c.IsPublic)
                .Select(c => new CategoryDTO()
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync(token);
            return result;
        }
        public async Task<List<CategoryDTO>> GetUsers(Guid userId, CancellationToken token)
        {
            var result = await _context.Categories
                .Where(c => c.UserId == userId && !c.IsDeleted)
                .Select(c => new CategoryDTO()
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync(token);
            return result;
        }
    }
}
