namespace EST.DAL.Models
{
    public class ItemExpense
    {
        public Guid ItemId { get; set; }
        public Item Item { get; set; }
        public Guid ExpenseId { get; set; }
        public Expense Expense { get; set; }
        public int Quantity { get; set; } 
    }
}
