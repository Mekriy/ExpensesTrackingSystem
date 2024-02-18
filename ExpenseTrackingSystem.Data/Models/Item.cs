using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DAL.Models
{
    public class Item : BaseEntity
    {
        public string Name { get; set; }
        public string Status { get; set; }
        public string Visibility { get; set; }
        public List<Review> Reviews { get; set; }
        public List<ItemExpense> ItemExpenses { get; set; }
    }
}
