using API.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DAL.DataAccess.Configurations
{
    public class ItemExpenseConfiguration : IEntityTypeConfiguration<ItemExpense>
    {
        public void Configure(EntityTypeBuilder<ItemExpense> builder)
        {
            builder
                .HasKey(ie => new { ie.ItemId, ie.ExpenseId });
            builder
                .HasOne(ie => ie.Item)
                .WithMany(ie => ie.ItemExpenses)
                .HasForeignKey(ie => ie.ItemId);
            builder
                .HasOne(ei => ei.Expense)
                .WithMany(ei => ei.ItemExpenses)
                .HasForeignKey(ei => ei.ExpenseId);
        }
    }
}
