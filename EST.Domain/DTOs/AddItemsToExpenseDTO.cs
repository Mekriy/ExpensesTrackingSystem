namespace EST.Domain.DTOs;

public class AddItemsToExpenseDTO
{
    public Guid ExpenseId { get; set; }
    public List<ItemIdDTO> Items { get; set; }
}