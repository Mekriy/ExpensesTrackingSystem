using EST.DAL.Models;
using EST.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EST.Domain.Pagination;

namespace EST.BL.Interfaces
{
    public interface IExpenseService
    {
        Task<PagedResponse<List<ExpenseDTO>>> GetAll(PaginationFilter filter, CancellationToken token);
        Task<PagedResponse<List<ExpenseDTO>>> GetAllUserExpenses(PaginationFilter filter, string user, CancellationToken token);
        Task<ExpenseDTO> GetById(Guid id, CancellationToken token);
        Task<ExpenseDTO> Create(ExpenseCreateDTO expenseDto,Guid userId, CancellationToken token);
        Task<ExpenseDTO> Update(ExpenseUpdateDTO expenseDto, Guid userId);
        Task<bool> Delete(Guid id);
        Task<bool> AddItems(Guid userId, Guid expenseId, List<ItemIdDTO> itemList);
        Task<List<ExpenseItemsDTO>> GetExpenseItems(Guid id, CancellationToken token);
        Task<bool> Exist(Guid id);
        Task<bool> SaveAsync();
    }
}
