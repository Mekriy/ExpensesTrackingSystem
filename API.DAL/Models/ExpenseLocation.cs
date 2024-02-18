using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DAL.Models
{
    public class ExpenseLocation
    {
        public Guid ExpenseId { get; set; }
        public Expense Expense { get; set; }
        public Guid LocationId { get; set; }
        public Location Location { get; set; }
    }
}
