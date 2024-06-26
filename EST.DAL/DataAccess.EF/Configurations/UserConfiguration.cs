﻿using EST.DAL.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace EST.DAL.DataAccess.EF.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder
                .HasKey(x => x.Id);
            builder
                .HasMany(e => e.Expenses)
                .WithOne(u => u.User);
            builder
                .HasMany(e => e.Categories)
                .WithOne(u => u.User)
                .OnDelete(DeleteBehavior.SetNull);
            builder
                .HasMany(r => r.Reviews)
                .WithOne(u => u.User)
                .OnDelete(DeleteBehavior.Cascade);
            builder
                .HasMany(i => i.Items)
                .WithOne(u => u.User)
                .HasForeignKey(u => u.UserId)
                .OnDelete(DeleteBehavior.SetNull);
            builder
                .HasMany(l => l.Locations)
                .WithOne(u => u.User)
                .HasForeignKey(u => u.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
