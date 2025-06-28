using Expenses.API.Dtos;
using Expenses.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Expenses.API.Data.Services;

public class TransactionsService(AppDbContext dbContext) : ITransactionsService
{
    public async Task<IEnumerable<Transaction>> GetAllTransactionsAsync(Guid userId)
    {
        var transactions = await dbContext.Transactions.Where(t => t.UserId == userId).ToListAsync();
        return transactions;
    }

    public Task<Transaction?> GetTransactionByIdAsync(Guid id)
    {
        var transaction = dbContext.Transactions.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
        return transaction;
    }

    public async Task<Transaction> CreateTransactionAsync(PostTransactionDto transactionDto, Guid userId)
    {
        var newTransaction = new Transaction
        {
            Type = transactionDto.Type,
            Category = transactionDto.Category,
            Description = transactionDto.Description,
            Amount = transactionDto.Amount,
            CreatedAt = DateTime.UtcNow,
            TransactionDate = transactionDto.TransactionDate,
            UserId = userId,
        };

         dbContext.Transactions.Add(newTransaction);
        try
        {
            dbContext.SaveChanges();
        }
        catch (DbUpdateException ex)
        {
            throw new Exception($"Internal server error: {ex.Message}");
        }

        return newTransaction;
    }

    public async Task<Transaction> UpdateTransactionAsync(Guid id, PostTransactionDto transactionDto)
    {
        var existingTransaction = await dbContext.Transactions.FirstOrDefaultAsync(t => t.Id == id);
        if (existingTransaction == null)
        {
            throw new KeyNotFoundException($"Transaction with ID {id} not found.");
        }

        existingTransaction.Type = transactionDto.Type;
        existingTransaction.Category = transactionDto.Category;
        existingTransaction.Description = transactionDto.Description;
        existingTransaction.Amount = transactionDto.Amount;
        existingTransaction.TransactionDate = transactionDto.TransactionDate;
        existingTransaction.UpdatedAt = DateTime.UtcNow;

        try
        {
            dbContext.SaveChanges();
        }
        catch (DbUpdateException ex)
        {
            throw new Exception($"Internal server error: {ex.Message}");
        }

        return existingTransaction;
    }

    public Task DeleteTransactionAsync(Guid id)
    {
        var transaction = dbContext.Transactions.FirstOrDefaultAsync(t => t.Id == id);
        if (transaction == null)
        {
            throw new KeyNotFoundException($"Transaction with ID {id} not found.");
        }

        dbContext.Transactions.Remove(transaction.Result);
        try
        {
            dbContext.SaveChanges();
        }
        catch (DbUpdateException ex)
        {
            throw new Exception($"Internal server error: {ex.Message}");
        }

        return Task.CompletedTask;
    }
}