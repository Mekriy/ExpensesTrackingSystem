namespace EST.DAL.Models
{
    public class ExpenseLocation
    {
        public Guid ExpenseId { get; set; }
        public Expense Expense { get; set; }
        public Guid LocationId { get; set; }
        public Location Location { get; set; }
    }
}
