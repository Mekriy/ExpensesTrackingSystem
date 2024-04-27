namespace EST.Domain.DTOs;

public class ItemExpenseDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int Price { get; set; }
    public int Quantity { get; set; }
    public double? Review { get; set; }
}