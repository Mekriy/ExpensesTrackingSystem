namespace EST.Domain.DTOs
{
    public class ExpenseCreateDTO
    {
        public int Price { get; set; }
        public Guid CategoryId { get; set; }
        public Guid LocationId { get; set; }
    }
}
