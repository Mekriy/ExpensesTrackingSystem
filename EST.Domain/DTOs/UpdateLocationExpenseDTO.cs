namespace EST.Domain.DTOs;

public class UpdateLocationExpenseDTO
{
    public Guid ExpenseId { get; set; }
    public Guid NewLocationId { get; set; }
    public Guid OldLocationId { get; set; }
}