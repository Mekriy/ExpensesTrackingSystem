using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EST.Domain.DTOs
{
    public class ExpenseCreateDTO
    {
        public int Price { get; set; }
        public string CategoryName { get; set; }
    }
}
