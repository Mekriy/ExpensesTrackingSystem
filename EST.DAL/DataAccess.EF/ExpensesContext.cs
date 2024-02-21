using API.DAL.DataAccess.Configurations;
using API.DAL.DataAccess.EF.Configurations;
using API.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace API.DAL.DataAccess
{
    public class ExpensesContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<ExpenseLocation> ExpensesLocations { get; set; }
        public DbSet<ItemExpense> ItemExpenses { get; set; }

        public ExpensesContext()
        {
        }
        public ExpensesContext(DbContextOptions<ExpensesContext> options) : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=localhost;Database=ExpensesTrackingSystemDB;Trusted_Connection=True;");
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
            modelBuilder.ApplyConfiguration(new ExpenseLocationConfiguration());
            modelBuilder.ApplyConfiguration(new ItemConfiguration());
            modelBuilder.ApplyConfiguration(new ItemExpenseConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new ReviewConfiguration());
            modelBuilder.ApplyConfiguration(new ExpenseConfiguration());
            modelBuilder.ApplyConfiguration(new LocationConfiguration());
            
            base.OnModelCreating(modelBuilder);
        }
    }
}
