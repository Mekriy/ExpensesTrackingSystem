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

        public void SeedReviews()
        {
            var itemsList = _context.Items.ToList();
            var usersList = _context.Users.ToList();

            var reviews = new List<Review>
            {
                new Review { Value = 5, ItemId = itemsList[0].Id, UserId = usersList[0].Id},
                new Review { Value = 2, ItemId = itemsList[1].Id, UserId = usersList[1].Id},
                new Review { Value = 10, ItemId = itemsList[2].Id, UserId = usersList[2].Id},
                new Review { Value = 3, ItemId = itemsList[3].Id, UserId = usersList[3].Id},
                new Review { Value = 8, ItemId = itemsList[4].Id, UserId = usersList[4].Id}
            };
            _context.Reviews.AddRange(reviews);
            _context.SaveChanges();
        }
        public void Seed()
        {
            var admin = new User { RoleName = "admin" };
            _context.Users.Add(admin);

            for (int i = 1; i <= 4; i++)
            {
                var user = new User { RoleName = "user" };
                _context.Users.Add(user);
            }

            _context.SaveChanges();

            var adminUser = _context.Users.FirstOrDefault(r => r.RoleName == "admin");

            // Seed Categories
            var categories = new List<Category>
            {
                new Category { Name = "Food", IsPublic = true, IsDeleted = false, UserId =  adminUser.Id},
                new Category { Name = "Entertainment", IsPublic = true, IsDeleted = false, UserId =  adminUser.Id},
                new Category { Name = "Transportation", IsPublic = true, IsDeleted = false, UserId =  adminUser.Id},
                new Category { Name = "Shopping", IsPublic = true, IsDeleted = false, UserId =  adminUser.Id},
                new Category { Name = "Utilities", IsPublic = true, IsDeleted = false, UserId =  adminUser.Id }
            };

            _context.Categories.AddRange(categories);
            _context.SaveChanges();

            var items = new List<Item>
            {
                new Item { Name = "Milk", IsPublic = false},
                new Item { Name = "Travel to Work", IsPublic = false},
                new Item { Name = "Pay Electricity Bill", IsPublic = false},
                new Item { Name = "Dine at Restaurant", IsPublic = false},
                new Item { Name = "Games", IsPublic = false}
            };
            _context.Items.AddRange(items);
            _context.SaveChanges();

            var itemsList = _context.Items.ToList();
            var usersList = _context.Users.ToList();

            var locations = new List<Location>
            {
                new Location { Name = "ATB", Latitude = "49.930665522850425", Longitude = "23.570525944141867", Address = "12 Stepana Bandery Street, Novoiavorivsk, Lviv Oblast, 81054"},
                new Location { Name = "Privat Bank", Latitude = "49.932198697820404", Longitude = "23.56916338206351", Address = "1 Stepana Bandery Street, Novoiavorivsk, Lviv Oblast, 81053"},
                new Location { Name = "WOG gas station", Latitude = "49.92592065881233", Longitude = "23.5735749452971", Address = "2 Lvivska Street, Novoiavorivsk, Lviv Oblast, 81053"},
                new Location { Name = "Lavanda restaurant", Latitude = "49.92766118720214", Longitude = "23.570721074895378", Address = "Sichovikh Striltsiv Street, Novoiavorivsk, Lviv Oblast, 81054"},
                new Location { Name = "Rukavychka", Latitude = "49.927896015577616", Longitude = "23.573135062999558", Address = "20 Sichovikh Striltsiv Street, Novoyavorivsk, Lviv Oblast, 81054"}
            };

            _context.Locations.AddRange(locations);
            _context.SaveChanges();

            var categoryLists = _context.Categories.ToList();

            var expenses = new List<Expense>
            {
                new Expense { Price = 300, Date = DateTime.Now.AddDays(3), CategoryId = categoryLists[0].Id, UserId = usersList[0].Id },
                new Expense { Price = 333, Date = DateTime.Now.AddDays(2), CategoryId = categoryLists[1].Id, UserId = usersList[1].Id },
                new Expense { Price = 555, Date = DateTime.Now.AddDays(1), CategoryId = categoryLists[2].Id, UserId = usersList[2].Id },
                new Expense { Price = 222, Date = DateTime.Now.AddDays(4), CategoryId = categoryLists[3].Id, UserId = usersList[3].Id },
                new Expense { Price = 10000, Date = DateTime.Now.AddDays(5), CategoryId = categoryLists[4].Id, UserId = usersList[4].Id }
            };

            _context.Expenses.AddRange(expenses);
            _context.SaveChanges();

            var expenseList = _context.Expenses.ToList();

            var itemexpenses = new List<ItemExpense>
            {
                new ItemExpense { ExpenseId = expenseList[0].Id, ItemId = itemsList[0].Id},
                new ItemExpense { ExpenseId = expenseList[1].Id, ItemId = itemsList[1].Id},
                new ItemExpense { ExpenseId = expenseList[1].Id, ItemId = itemsList[2].Id},
                new ItemExpense { ExpenseId = expenseList[1].Id, ItemId = itemsList[3].Id},
                new ItemExpense { ExpenseId = expenseList[2].Id, ItemId = itemsList[2].Id},
                new ItemExpense { ExpenseId = expenseList[3].Id, ItemId = itemsList[3].Id},
                new ItemExpense { ExpenseId = expenseList[3].Id, ItemId = itemsList[1].Id},
                new ItemExpense { ExpenseId = expenseList[3].Id, ItemId = itemsList[2].Id},
                new ItemExpense { ExpenseId = expenseList[4].Id, ItemId = itemsList[4].Id}
            };

            _context.ItemExpenses.AddRange(itemexpenses);
            _context.SaveChanges();

            var locationList = _context.Locations.ToList();

            var expenseLocation = new List<ExpenseLocation>
            {
                new ExpenseLocation { ExpenseId = expenseList[0].Id, LocationId = locationList[0].Id},
                new ExpenseLocation { ExpenseId = expenseList[1].Id, LocationId = locationList[1].Id},
                new ExpenseLocation { ExpenseId = expenseList[2].Id, LocationId = locationList[2].Id},
                new ExpenseLocation { ExpenseId = expenseList[3].Id, LocationId = locationList[3].Id},
                new ExpenseLocation { ExpenseId = expenseList[4].Id, LocationId = locationList[4].Id}
            };

            _context.ExpensesLocations.AddRange(expenseLocation);
            _context.SaveChanges();
        }
    }
}
