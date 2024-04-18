namespace EST.DAL.Models
{
    public class Location : BaseEntity
    {
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Address { get; set; }
        public bool Save { get; set; }
        public Guid? UserId { get; set; }
        public User? User { get; set; }
        public List<Expense> Expenses { get; set; }    }
}
