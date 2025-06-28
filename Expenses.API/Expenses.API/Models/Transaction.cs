namespace Expenses.API.Models;

public class Transaction : Base.BaseEntity
{
    public string Type { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
    public Guid? UserId { get; set; }
    public virtual User? User { get; set; }
}