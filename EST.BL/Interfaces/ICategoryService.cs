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
        Task<bool> Create(CategoryDTO categoryDto);
        Task<bool> Update(UpdateCategoryDTO categoryDto);
        Task<bool> Delete(Guid id);
        Task<bool> Exist(string name);
        Task<bool> Exist(Guid id);
        Task<bool> SaveAsync();
    }
}
