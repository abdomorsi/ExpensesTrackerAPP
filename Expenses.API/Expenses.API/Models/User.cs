namespace Expenses.API.Models;

public class User : Base.BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public List<Transaction> Transactions { get; set; } = new List<Transaction>();
}