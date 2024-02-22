using EST.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EST.BL.Interfaces
{
    public interface IExpenseService : IBaseService<Expense>
    {
        Task<List<Expense>> GetAll();
    }
}
