namespace EST.Domain.DTOs;

public class ExpenseDTO
{
    public Guid Id { get; set; }
    public int Price {  get; set; }
    public DateTime Date { get; set; } 
    public string CategoryName { get; set; }
}