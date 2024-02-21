namespace API.DAL.Models
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public List<Expense> Expenses { get; set; }
    }
}
