using EST.DAL.Models;
using EST.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EST.BL.Interfaces
{
    public interface IExpenseService
    {
        Task<List<Expense>> GetAll(CancellationToken token);
        Task<Expense> GetById(Guid id, CancellationToken token);
        Task<ExpenseDTO> Create(ExpenseCreateDTO expenseDto, CancellationToken token);
        Task<ExpenseDTO> Update(ExpenseUpdateDTO expenseDto, Guid userId);
        Task<bool> Delete(Guid id);
        Task<bool> AddItems(Guid id, List<ItemIdDTO> itemList);
        Task<List<ExpenseItemsDTO>> GetExpenseItems(Guid id, CancellationToken token);
        Task<bool> Exist(Guid id);
        Task<bool> SaveAsync();
    }
}
