﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EST.Domain.DTOs
{
    public class ExpenseDTO
    {
        public int Price { get; set; }
        public DateTime Date { get; set; }
        public Guid UserId { get; set; }
        public Guid CategoryId { get; set; }
    }
}