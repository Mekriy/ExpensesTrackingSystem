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
        Task<PagedResponse<List<PaginationExpenseItemsDTO>>> GetAll(PaginationFilter filter, CancellationToken token);
        Task<PagedResponse<List<PaginationExpenseItemsDTO>>> GetAllUserExpenses(PaginationFilter filter, string user, CancellationToken token);
        Task<ExpenseDTO> GetById(Guid id, CancellationToken token);
        Task<ExpenseDTO> Create(ExpenseCreateDTO expenseDto,Guid userId, CancellationToken token);
        Task<ExpenseDTO> Update(ExpenseUpdateDTO expenseDto, Guid userId);
        Task<bool> Delete(Guid id);
        Task<bool> AddItems(Guid userId, AddItemsToExpenseDTO itemList);
        Task<List<ExpenseItemsDTO>> GetExpenseItems(Guid id, CancellationToken token);
        Task<List<CountExpensesByCategoryDTO>> GetExpensesCountByCategory(Guid userId);
        Task<List<LastFiveExpensesDTO>> GetLastFiveExpenses(Guid userId);
        Task<MonthlyOverviewDTO> GetMonthlyOverview(Guid userId, CancellationToken token);
    }
}
