

namespace EST.Domain.DTOs
{
    public class ReviewDTO
    {
        public Guid ItemId { get; set; }
        public double Value { get; set; }
        public Guid UserId { get; set; }
    }
}
