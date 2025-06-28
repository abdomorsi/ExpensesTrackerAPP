using Expenses.API.Dtos;
using Expenses.API.Models;

namespace Expenses.API.Data.Services;

public interface ITransactionsService
{
    Task<IEnumerable<Transaction>> GetAllTransactionsAsync(Guid userId);
    Task<Transaction?> GetTransactionByIdAsync(Guid id);
    Task<Transaction> CreateTransactionAsync(PostTransactionDto transactionDto, Guid userId);
    Task<Transaction> UpdateTransactionAsync(Guid id, PostTransactionDto transactionDto);
    Task DeleteTransactionAsync(Guid id);
}