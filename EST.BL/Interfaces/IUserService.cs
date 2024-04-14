using EST.DAL.Models;
using EST.Domain.DTOs;

namespace EST.BL.Interfaces
{
    public interface IUserService
    {
        Task<UserWithPhotoDTO?> GetById(Guid id, CancellationToken token);
        Task<UserDTO> Create(CreateUserDTO user);
        Task<bool> Delete(Guid id, CancellationToken token);
        Task<bool> Exist(Guid id);
        Task<bool> SaveAsync();
    }
}
