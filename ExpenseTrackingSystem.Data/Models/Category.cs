using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DAL.Models
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public List<Expense> Expenses { get; set; }
    }
}
