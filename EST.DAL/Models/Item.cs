namespace EST.DAL.Models
{
    public class Item : BaseEntity
    {
        public string Name { get; set; }
        public bool IsPublic { get; set; }
        public List<Review> Reviews { get; set; }
        public List<ItemExpense> ItemExpenses { get; set; }
    }
}
