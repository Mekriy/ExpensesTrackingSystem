namespace EST.Domain.DTOs;

public class ExpenseUpdateDTO
{
    public Guid Id { get; set; }
    public int Price { get; set; }
    public DateTime Date { get; set; }
    public Guid CategoryId { get; set; }
}