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
        Task<List<Expense>> GetAll();
        Task<Expense> GetById(Guid id);
        Task<bool> Create(ExpenseDTO expenseDto);
        Task<bool> Update(ExpenseDTO expenseDto);
        Task<bool> Delete(Guid id);
        Task<bool> Exist(Guid id);
        Task<bool> SaveAsync();
    }
}
