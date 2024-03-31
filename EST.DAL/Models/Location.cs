namespace EST.DAL.Models
{
    public class Location : BaseEntity
    {
        public string Name { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Address { get; set; }
        public bool Save { get; set; }
        public Guid? UserId { get; set; }
        public User? User { get; set; }
        public List<ExpenseLocation> ExpenseLocations { get; set; }
    }
}
