using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DAL.Models
{
    public class ItemExpense
    {
        public Guid ItemId { get; set; }
        public Item Item { get; set; }
        public Guid ExpenseId { get; set; }
        public Expense Expense { get; set; }
    }
}
