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
    public class UserService : BaseService<User>, IUserService
    {
        private readonly ExpensesContext _expensesContext;
        public UserService(ExpensesContext expensesContext) : base(expensesContext)
        {
            _expensesContext = expensesContext;
        }

        public async Task<List<User>> GetAll()
        {
            return await _expensesContext.Users.ToListAsync();
        }
    }
}
