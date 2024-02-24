using EST.DAL.Models;
using EST.Domain.DTOs;

namespace EST.BL.Interfaces
{
    public interface IUserService
    {
        Task<User> GetById(Guid id);
        Task<bool> Create(UserDTO user);
        Task<bool> Update(UserDTO user);
        Task<bool> Delete(Guid id);
        Task<bool> Exist(Guid id);
        Task<bool> Exist(string name);
        Task<bool> SaveAsync();
        Task<List<User>> GetAll();
    }
}
