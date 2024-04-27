namespace EST.DAL.Models
{
    public class Review : BaseEntity
    {
        public double Value { get; set; }
        public Guid ItemId { get; set; }
        public Item Item { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
