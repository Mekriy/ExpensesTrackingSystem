using EST.DAL.Models;
using EST.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EST.BL.Interfaces
{
    public interface ICategoryService
    {
        Task<Category> GetById(Guid id, CancellationToken token); 
        Task<CategoryDTO> Create(CreateCategoryDTO categoryDto, Guid userId, CancellationToken token);
        Task<bool> Update(UpdateCategoryDTO categoryDto, Guid userId, CancellationToken token);
        Task<bool> Delete(string name, Guid userId, CancellationToken token);
        Task<bool> Exist(string name, Guid userId, CancellationToken token);
        Task<List<CategoryDTO>> GetPublic(CancellationToken token);
        Task<List<CategoryDTO>> GetUsers(Guid userId, CancellationToken token);
    }
}
