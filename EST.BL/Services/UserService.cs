using EST.BL.Interfaces;
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
        public async Task<User> GetById(Guid Id)
        {
            return await _expensesContext.Users.Where(u => u.Id == Id).FirstOrDefaultAsync();
        }
        public async Task<bool> Create(UserDTO userDto)
        {
            var user = new User
            {
                Name = userDto.Name,
                Email = userDto.Email,
                Password = userDto.Password,
            };
            await _expensesContext.Users.AddAsync(user);
            return await SaveAsync();
        }
        //do i need this? lol
        public async Task<bool> Update(UserDTO userDTO)
        {
            var user = await _expensesContext.Users.Where(i => i.Email == userDTO.Email).FirstOrDefaultAsync();
            if (user == null)
                return false;
            user = new User()
            {
                Name = userDTO.Name,
                Email = userDTO.Email,
                Password = userDTO.Password,
            };
            _expensesContext.Users.Update(user);
            return await SaveAsync();
        }
        public async Task<bool> Delete(Guid Id)
        {
            if (!await Exist(Id))
            {
                return false;
            }
            var user = await GetById(Id);
            _expensesContext.Users.Remove(user);
            return await SaveAsync();
        }
        public async Task<bool> Exist(Guid id)
        {
            return await _expensesContext.Users.Where(u => u.Id == id).AnyAsync();
        }
        public async Task<bool> Exist(string name)
        {
            return await _expensesContext.Users.Where(u => u.Name == name).AnyAsync();
        }
        public async Task<bool> SaveAsync()
        {
            var saved = await _expensesContext.SaveChangesAsync();
            return saved > 0 ? true : false;
        }
        public async Task<List<User>> GetAll()
        {
            return await _expensesContext.Users.ToListAsync();
        }
    }
}
