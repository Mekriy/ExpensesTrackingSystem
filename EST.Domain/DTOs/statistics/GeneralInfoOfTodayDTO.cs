namespace EST.Domain.DTOs;

public class GeneralInfoOfTodayDTO
{
    public string CategoryName { get; set; }
    public int Count { get; set; }
    public int AllExpensesCount { get; set; }
    public double AverageExpensePrice { get; set; }
}