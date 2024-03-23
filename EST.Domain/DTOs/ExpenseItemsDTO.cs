﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EST.Domain.DTOs
{
    public class ExpenseItemsDTO
    {
        public string Name { get; set; }
        public int Price { get; set; }
        public bool IsPublic { get; set; }
        public int Quantity { get; set; }
    }
}
