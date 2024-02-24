namespace EST.DAL.Models
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }
        public bool IsPublic { get; set; }
        public bool IsDeleted { get; set; }
        public User? User { get; set; }
        public Guid? UserId { get; set; }
        public List<Expense> Expenses { get; set; }
    }
}
