using EST.DAL.DataAccess.EF.Configurations;
using EST.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace EST.DAL.DataAccess.EF
{
    public class ExpensesContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<ItemExpense> ItemExpenses { get; set; }
        public DbSet<PhotoFile> PhotoFiles { get; set; }

        public ExpensesContext()
        {
        }
        public ExpensesContext(DbContextOptions<ExpensesContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
            modelBuilder.ApplyConfiguration(new ItemConfiguration());
            modelBuilder.ApplyConfiguration(new ItemExpenseConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new ReviewConfiguration());
            modelBuilder.ApplyConfiguration(new ExpenseConfiguration());
            modelBuilder.ApplyConfiguration(new LocationConfiguration());
            modelBuilder.ApplyConfiguration(new PhotoFileConfiguration());
            
            base.OnModelCreating(modelBuilder);
        }
    }
}
