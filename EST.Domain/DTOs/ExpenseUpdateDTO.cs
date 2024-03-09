namespace EST.Domain.DTOs;

public class ExpenseUpdateDTO
{
    public int Price { get; set; }
    public DateTime Date { get; set; }
    public Guid CategoryId { get; set; }
}