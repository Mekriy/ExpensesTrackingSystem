using EST.DAL.DataAccess.EF;
using EST.DAL.Models;

namespace ETS.WebAPI
{
    public class DbSeeder
    {
        private readonly ExpensesContext _context;
        public DbSeeder(ExpensesContext context)
        {
            _context = context;
        }

        public void Seed()
        {
            var admin = new User { Name = "Admin", Email = "admin@example.com", Password = "admin123", RoleName = "admin" };
            _context.Users.Add(admin);

            for (int i = 1; i <= 4; i++)
            {
                var user = new User { Name = $"User {i}", Email = $"user{i}@example.com", Password = $"user{i}pass", RoleName = "user" };
                _context.Users.Add(user);
            }

            _context.SaveChanges();

            // Seed Categories
            var categories = new List<Category>
        {
            new Category { Name = "Food" },
            new Category { Name = "Entertainment" },
            new Category { Name = "Transportation" },
            new Category { Name = "Shopping" },
            new Category { Name = "Utilities" }
        };

            _context.Categories.AddRange(categories);
            _context.SaveChanges();

            // Seed Expenses
            var random = new Random();
            foreach (var user in _context.Users)
            {
                foreach (var category in _context.Categories)
                {
                    var expense = new Expense
                    {
                        Price = random.Next(10, 100),
                        Date = DateTime.Now.AddDays(-random.Next(1, 30)),
                        UserId = user.Id,
                        CategoryId = category.Id
                    };

                    // Seed ItemExpenses (2-3 items per expense)
                    var items = _context.Items.OrderBy(x => Guid.NewGuid()).Take(random.Next(2, 4)).ToList();
                    foreach (var item in items)
                    {
                        expense.ItemExpenses.Add(new ItemExpense { ItemId = item.Id });
                    }

                    // Seed ExpenseLocations (1-2 locations per expense)
                    var locations = _context.Locations.OrderBy(x => Guid.NewGuid()).Take(random.Next(1, 3)).ToList();
                    foreach (var location in locations)
                    {
                        expense.ExpenseLocations.Add(new ExpenseLocation { LocationId = location.Id });
                    }

                    _context.Expenses.Add(expense);
                }
            }

            _context.SaveChanges();

            // Seed Reviews
            foreach (var item in _context.Items)
            {
                for (int i = 0; i < 5; i++)
                {
                    var review = new Review { Value = random.Next(1, 6), ItemId = item.Id };
                    _context.Reviews.Add(review);
                }
            }

            _context.SaveChanges();
        }
    }
}
