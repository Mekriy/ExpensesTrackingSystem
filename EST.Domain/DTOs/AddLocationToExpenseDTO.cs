namespace EST.Domain.DTOs;

public class AddLocationToExpenseDTO
{
    public Guid LocationId { get; set; }
    public Guid ExpenseId { get; set; }
}