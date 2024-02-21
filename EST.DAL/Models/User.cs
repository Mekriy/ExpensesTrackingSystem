namespace API.DAL.Models
{
    public class User : BaseEntity
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string RoleName { get; set; }
        public List<Expense> Expenses { get; set; }
    }
}