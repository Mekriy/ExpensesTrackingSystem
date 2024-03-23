namespace EST.Domain.DTOs;

public class PaginationExpenseItemsDTO
{
    public int Price { get; set; }
    public DateTime Date { get; set; }
    public List<string> Name { get; set; }
    public List<int> Quantity { get; set; }
}