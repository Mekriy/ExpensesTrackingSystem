namespace EST.DAL.Models
{
    public class User : BaseEntity
    {
        public string RoleName { get; set; }
        public List<Expense> Expenses { get; set; }
        public List<Category> Categories { get; set; }
        public List<Review> Reviews { get; set; }
        public List<Item> Items { get; set; }
        public PhotoFile PhotoFile { get; set; }
    }
}