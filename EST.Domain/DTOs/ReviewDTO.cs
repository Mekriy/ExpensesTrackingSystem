﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EST.Domain.DTOs
{
    public class ReviewDTO
    {
        public Guid ItemId { get; set; }
        public double Value { get; set; }
        public Guid UserId { get; set; }
    }
}
