namespace EST.Domain.DTOs;

public class PaginationExpenseItemsDTO
{
    public Guid Id { get; set; }
    public int Price { get; set; }
    public DateTime Date { get; set; }
    public string CategoryName { get; set; }
    public LocationDTO Location { get; set; }
    public List<ItemExpenseDTO> Items { get; set; }
}