﻿// <auto-generated />
using System;
using EST.DAL.DataAccess.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace EST.DAL.Migrations
{
    [DbContext(typeof(ExpensesContext))]
    [Migration("20240402082735_ChangedValueTypeInReviewTable")]
    partial class ChangedValueTypeInReviewTable
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.27")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("EST.DAL.Models.Category", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<bool>("IsPublic")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("EST.DAL.Models.Expense", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CategoryId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<int>("Price")
                        .HasColumnType("int");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("UserId");

                    b.ToTable("Expenses");
                });

            modelBuilder.Entity("EST.DAL.Models.ExpenseLocation", b =>
                {
                    b.Property<Guid>("ExpenseId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("LocationId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("ExpenseId", "LocationId");

                    b.HasIndex("LocationId");

                    b.ToTable("ExpensesLocations");
                });

            modelBuilder.Entity("EST.DAL.Models.Item", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<bool>("IsPublic")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Price")
                        .HasColumnType("int");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Items");
                });

            modelBuilder.Entity("EST.DAL.Models.ItemExpense", b =>
                {
                    b.Property<Guid>("ItemId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ExpenseId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("ItemId", "ExpenseId");

                    b.HasIndex("ExpenseId");

                    b.ToTable("ItemExpenses");
                });

            modelBuilder.Entity("EST.DAL.Models.Location", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Latitude")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Longitude")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Save")
                        .HasColumnType("bit");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Locations");
                });

            modelBuilder.Entity("EST.DAL.Models.PhotoFile", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FilePath")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("PhotoFiles");
                });

            modelBuilder.Entity("EST.DAL.Models.Review", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ItemId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<double>("Value")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.HasIndex("ItemId");

                    b.HasIndex("UserId");

                    b.ToTable("Reviews");
                });

            modelBuilder.Entity("EST.DAL.Models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("EST.DAL.Models.Category", b =>
                {
                    b.HasOne("EST.DAL.Models.User", "User")
                        .WithMany("Categories")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("User");
                });

            modelBuilder.Entity("EST.DAL.Models.Expense", b =>
                {
                    b.HasOne("EST.DAL.Models.Category", "Category")
                        .WithMany("Expenses")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EST.DAL.Models.User", "User")
                        .WithMany("Expenses")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");

                    b.Navigation("User");
                });

            modelBuilder.Entity("EST.DAL.Models.ExpenseLocation", b =>
                {
                    b.HasOne("EST.DAL.Models.Expense", "Expense")
                        .WithMany("ExpenseLocations")
                        .HasForeignKey("ExpenseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EST.DAL.Models.Location", "Location")
                        .WithMany("ExpenseLocations")
                        .HasForeignKey("LocationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Expense");

                    b.Navigation("Location");
                });

            modelBuilder.Entity("EST.DAL.Models.Item", b =>
                {
                    b.HasOne("EST.DAL.Models.User", "User")
                        .WithMany("Items")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("User");
                });

            modelBuilder.Entity("EST.DAL.Models.ItemExpense", b =>
                {
                    b.HasOne("EST.DAL.Models.Expense", "Expense")
                        .WithMany("ItemExpenses")
                        .HasForeignKey("ExpenseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EST.DAL.Models.Item", "Item")
                        .WithMany("ItemExpenses")
                        .HasForeignKey("ItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Expense");

                    b.Navigation("Item");
                });

            modelBuilder.Entity("EST.DAL.Models.Location", b =>
                {
                    b.HasOne("EST.DAL.Models.User", "User")
                        .WithMany("Locations")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("User");
                });

            modelBuilder.Entity("EST.DAL.Models.PhotoFile", b =>
                {
                    b.HasOne("EST.DAL.Models.User", "User")
                        .WithOne("PhotoFile")
                        .HasForeignKey("EST.DAL.Models.PhotoFile", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("EST.DAL.Models.Review", b =>
                {
                    b.HasOne("EST.DAL.Models.Item", "Item")
                        .WithMany("Reviews")
                        .HasForeignKey("ItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EST.DAL.Models.User", "User")
                        .WithMany("Reviews")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Item");

                    b.Navigation("User");
                });

            modelBuilder.Entity("EST.DAL.Models.Category", b =>
                {
                    b.Navigation("Expenses");
                });

            modelBuilder.Entity("EST.DAL.Models.Expense", b =>
                {
                    b.Navigation("ExpenseLocations");

                    b.Navigation("ItemExpenses");
                });

            modelBuilder.Entity("EST.DAL.Models.Item", b =>
                {
                    b.Navigation("ItemExpenses");

                    b.Navigation("Reviews");
                });

            modelBuilder.Entity("EST.DAL.Models.Location", b =>
                {
                    b.Navigation("ExpenseLocations");
                });

            modelBuilder.Entity("EST.DAL.Models.User", b =>
                {
                    b.Navigation("Categories");

                    b.Navigation("Expenses");

                    b.Navigation("Items");

                    b.Navigation("Locations");

                    b.Navigation("PhotoFile")
                        .IsRequired();

                    b.Navigation("Reviews");
                });
#pragma warning restore 612, 618
        }
    }
}
