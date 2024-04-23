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
        private readonly ExpensesContext _context;
        public UserService(ExpensesContext context)
        {
            _context = context;
        }
        public async Task<UserWithPhotoDTO?> GetById(Guid userId, CancellationToken token)
        {
            return await _context.Users
                .Include(p => p.PhotoFile)
                .Where(u => u.Id == userId)
                .Select(x => new UserWithPhotoDTO()
                {
                    Id = x.Id,
                    Email = x.Email,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    FileName = x.PhotoFile.FileName,
                    RoleName = x.RoleName
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
            await _context.Users.AddAsync(user);
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

            var user = await _context.Users.Where(u => u.Id == Id).FirstOrDefaultAsync(token);
            var result = await PrivateItemDeleteWithUser(user.Id);
            _context.Users.Remove(user);
            return await SaveAsync();
        }
        private async Task<bool> PrivateItemDeleteWithUser(Guid userId)
        {
            var items = await _context.Items
                .Where(i => !i.IsPublic && i.UserId == userId)
                .ToListAsync();
            _context.Items.RemoveRange(items);
            return await SaveAsync();
        }
        public async Task<bool> Exist(Guid id)
        {
            return await _context.Users.Where(u => u.Id == id).AnyAsync();
        }
        public async Task<bool> SaveAsync()
        {
            var saved = await _context.SaveChangesAsync();
            return saved > 0 ? true : false;
        }

        public async Task<UsersCreatedInfoDTO> GetUserCreatedInfo(Guid userId, CancellationToken token)
        {
            var result = await _context.Users.Select(t => new
            {
                t.Id,
                items = _context.Items
                    .Where(i => i.UserId == t.Id)
                    .Include(r => r.Reviews)
                    .Select(it => new ItemDTO()
                    {
                        Id = it.Id,
                        Name = it.Name,
                        Price = it.Price,
                        Value = it.Reviews.Any(r => r.ItemId == it.Id) ? 
                            it.Reviews.Where(r => r.ItemId == it.Id).Average(rv => rv.Value) : 0,
                    })
                    .ToList(),
                categories = _context.Categories
                    .Where(c => c.UserId == t.Id)
                    .Select(ct => new CategoryDTO()
                    {
                        Id = ct.Id,
                        Name = ct.Name
                    })
                    .ToList(),
                locations = _context.Locations
                    .Where(l => l.UserId == t.Id && l.Save == true)
                    .Select(lt => new LocationDTO()
                    {
                        Name = lt.Name,
                        Latitude = lt.Latitude,
                        Longitude = lt.Longitude,
                        Address = lt.Address,
                        Save = lt.Save,
                    })
                    .ToList()
            }).FirstOrDefaultAsync(u => u.Id == userId, token);
            
            return new UsersCreatedInfoDTO()
            {
                Items = result.items,
                Categories = result.categories,
                Locations = result.locations
            };
        }

        public async Task<UserDTO> UpdateFullName(Guid id, UpdateUserFullNameDTO fullName, CancellationToken token)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id, token);
            user.FirstName = fullName.FirstName;
            user.LastName = fullName.LastName;

            _context.Update(user);
            if (await _context.SaveChangesAsync(token) > 0)
            {
                return new UserDTO()
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                };
            }
            throw new ApiException()
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Title = "Can't update user",
                Detail = "Error occured while updating user's full name"
            };
        }
    }
}
