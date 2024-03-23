namespace EST.DAL.Models
{
    public class Item : BaseEntity
    {
        public string Name { get; set; }
        public int Price { get; set; }
        public bool IsPublic { get; set; }
        public bool IsDeleted { get; set; }
        public List<Review> Reviews { get; set; }
        public List<ItemExpense> ItemExpenses { get; set; }
        public Guid? UserId { get; set; }
        public User? User { get; set; }
    }
}
