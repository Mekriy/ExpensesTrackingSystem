namespace EST.Domain.DTOs;

public class PaginationExpenseItemsDTO
{
    public Guid Id { get; set; }
    public int Price { get; set; }
    public DateTime Date { get; set; }
    public CategoryDTO Category { get; set; }
    public DropdownLocationDTO Location { get; set; }
    public List<ItemExpenseDTO> Items { get; set; }
}