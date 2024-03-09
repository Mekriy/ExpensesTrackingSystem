﻿using EST.BL.Interfaces;
using EST.DAL.DataAccess.EF;
using EST.DAL.Models;
using EST.Domain.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace EST.BL.Services
{
    public class UserService : IUserService
    {
        private readonly ExpensesContext _expensesContext;
        public UserService(ExpensesContext expensesContext)
        {
            _expensesContext = expensesContext;
        }
        public async Task<UserDTO> GetById(Guid userId, CancellationToken token)
        {
            var user = await _expensesContext.Users.Where(u => u.Id == userId).FirstOrDefaultAsync(token);
            var userDTO = new UserDTO()
            {
                Id = user.Id,
                RoleName = user.RoleName,
            };
            return userDTO;
        }
        public async Task<bool> Create(UserDTO userDTO)
        {
            var user = new User
            {
                Id = userDTO.Id,
                RoleName = userDTO.RoleName,
            };
            await _expensesContext.Users.AddAsync(user);
            return await SaveAsync();
        }
        public async Task<bool> Delete(Guid Id, CancellationToken token)
        {
            if (!await Exist(Id))
            {
                return false;
            }

            var user = await _expensesContext.Users.Where(u => u.Id == Id).FirstOrDefaultAsync(token);
            _expensesContext.Users.Remove(user);
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
