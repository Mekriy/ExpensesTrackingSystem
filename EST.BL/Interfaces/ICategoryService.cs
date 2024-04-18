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
        Task<CategoryDTO> Create(CreateCategoryDTO categoryDto, Guid userId);
        Task<bool> Update(UpdateCategoryDTO categoryDto);
        Task<bool> Delete(Guid id);
        Task<bool> Exist(string name);
        Task<bool> Exist(Guid id);
        Task<bool> SaveAsync();
        Task<List<CategoryDTO>> GetPublic();
        Task<List<CategoryDTO>> GetUsers(Guid userId);
    }
}
