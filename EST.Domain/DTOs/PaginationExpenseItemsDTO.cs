namespace EST.Domain.DTOs;

public class PaginationExpenseItemsDTO
{
    public int Price { get; set; }
    public DateTime Date { get; set; }
    public List<ItemExpenseDTO> Items { get; set; }
}