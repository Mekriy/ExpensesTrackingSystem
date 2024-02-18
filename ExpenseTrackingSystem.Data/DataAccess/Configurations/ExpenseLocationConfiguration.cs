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
    public class ExpenseLocationConfiguration : IEntityTypeConfiguration<ExpenseLocation>
    {
        public void Configure(EntityTypeBuilder<ExpenseLocation> builder)
        {
            builder
                .HasKey(el => new { el.ExpenseId, el.LocationId });
            builder
                .HasOne(el => el.Expense)
                .WithMany(el => el.ExpenseLocations)
                .HasForeignKey(el => el.ExpenseId);
            builder
                .HasOne(el => el.Location)
                .WithMany(el => el.ExpenseLocations)
                .HasForeignKey(el => el.LocationId);
        }
    }
}
