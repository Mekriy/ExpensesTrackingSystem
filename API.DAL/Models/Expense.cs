using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DAL.Models
{
    public class Expense : BaseEntity
    {
        public int Price {  get; set; }
        public DateTime Date { get; set; }
        
        
        public Guid UserId { get; set; }
        public User User { get; set; }
        public Guid CategoryId { get; set; }
        public Category Category { get; set; }
        public List<ItemExpense> ItemExpenses { get; set; }
        public List<ExpenseLocation> ExpenseLocations { get; set; }
    }
}
