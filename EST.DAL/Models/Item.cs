namespace EST.DAL.Models
{
    public class Item : BaseEntity
    {
        public string Name { get; set; }
        public string Status { get; set; }
        public string Visibility { get; set; }
        public List<Review> Reviews { get; set; }
        public List<ItemExpense> ItemExpenses { get; set; }
    }
}
