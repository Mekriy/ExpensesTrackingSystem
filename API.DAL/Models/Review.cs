using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DAL.Models
{
    public class Review : BaseEntity
    {
        public int Value { get; set; }
        public Guid ItemId { get; set; }
        public Item Item { get; set; }
    }
}
