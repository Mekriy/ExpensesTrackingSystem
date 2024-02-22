namespace EST.DAL.Models
{
    public class Review : BaseEntity
    {
        public int Value { get; set; }
        public Guid ItemId { get; set; }
        public Item Item { get; set; }
    }
}
