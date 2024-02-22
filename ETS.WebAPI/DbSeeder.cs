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
            var users = new List<User>
            {

            };
        }
    }
}
