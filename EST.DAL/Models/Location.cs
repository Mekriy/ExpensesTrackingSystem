namespace API.DAL.Models
{
    public class Location : BaseEntity
    {
        public string Name { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Address { get; set; }
        public List<ExpenseLocation> ExpenseLocations { get; set; }
    }
}
