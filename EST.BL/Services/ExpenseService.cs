using EST.BL.Interfaces;
using EST.DAL.DataAccess.EF;
using EST.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EST.BL.Services
{
    public class ExpenseService : BaseService<Expense>, IExpenseService
    {
        private readonly ExpensesContext _expensesContext;
        public ExpenseService(ExpensesContext expensesContext) : base(expensesContext)
        {
            _expensesContext = expensesContext;
        }

        public async Task<List<Expense>> GetAll()
        {
            return await _expensesContext.Expenses.ToListAsync();
        }
    }
}
