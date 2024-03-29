﻿using EST.DAL.Models;
using EST.Domain.DTOs;

namespace EST.BL.Interfaces
{
    public interface IUserService
    {
        Task<UserDTO> GetById(Guid id, CancellationToken token);
        Task<UserDTO> Create(UserDTO user);
        Task<bool> Delete(Guid id, CancellationToken token);
        Task<bool> Exist(Guid id);
        Task<bool> SaveAsync();
    }
}
