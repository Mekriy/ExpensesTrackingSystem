using EST.BL.Interfaces;
using EST.DAL.DataAccess.EF;
using EST.DAL.Models;
using EST.Domain.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;
using EST.Domain.Helpers;
using Microsoft.AspNetCore.Http;

namespace EST.BL.Services
{
    public class UserService : IUserService
    {
        private readonly ExpensesContext _expensesContext;
        public UserService(ExpensesContext expensesContext)
        {
            _expensesContext = expensesContext;
        }
        public async Task<UserWithPhotoDTO?> GetById(Guid userId, CancellationToken token)
        {
            return await _expensesContext.Users
                .Include(p => p.PhotoFile)
                .Where(u => u.Id == userId)
                .Select(x => new UserWithPhotoDTO()
                {
                    Email = x.Email,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    FileName = x.PhotoFile.FileName
                }).FirstOrDefaultAsync(token);
        }
        public async Task<UserDTO> Create(CreateUserDTO userDTO)
        {
            var user = new User
            {
                Id = userDTO.Id,
                Email = userDTO.Email,
                FirstName = userDTO.FirstName,
                LastName = userDTO.LastName,
                RoleName = userDTO.RoleName,
            };
            await _expensesContext.Users.AddAsync(user);
            var result = await SaveAsync();
            if (result)
                return new UserDTO()
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                };
            return null;
        }
        public async Task<bool> Delete(Guid Id, CancellationToken token)
        {
            if (!await Exist(Id))
            {
                return false;
            }

            var user = await _expensesContext.Users.Where(u => u.Id == Id).FirstOrDefaultAsync(token);
            var result = await PrivateItemDeleteWithUser(user.Id);
            _expensesContext.Users.Remove(user);
            return await SaveAsync();
        }
        private async Task<bool> PrivateItemDeleteWithUser(Guid userId)
        {
            var items = await _expensesContext.Items
                .Where(i => !i.IsPublic && i.UserId == userId)
                .ToListAsync();
            _expensesContext.Items.RemoveRange(items);
            return await SaveAsync();
        }
        public async Task<bool> Exist(Guid id)
        {
            return await _expensesContext.Users.Where(u => u.Id == id).AnyAsync();
        }
        public async Task<bool> SaveAsync()
        {
            var saved = await _expensesContext.SaveChangesAsync();
            return saved > 0 ? true : false;
        }
    }
}
