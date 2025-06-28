using System.Security.Claims;
using Expenses.API.Data;
using Expenses.API.Data.Services;
using Expenses.API.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Transaction = Expenses.API.Models.Transaction;

namespace Expenses.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class TransactionsController(ITransactionsService service) : ControllerBase
{
    [HttpGet("All")]
    public IActionResult GetTransactions()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new { Message = "User ID is required." });
            }
            
            var transactions = service.GetAllTransactionsAsync(Guid.Parse(userId)).Result;
            if (transactions == null || !transactions.Any())
            {
                return NotFound(new { Message = "No transactions found." });
            }
            return Ok(transactions);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("{id}")]
    public IActionResult GetTransaction(Guid id)
    {
        try
        {
            var transaction = service.GetTransactionByIdAsync(id).Result;
            if (transaction == null)
            {
                return NotFound(new { Message = $"Transaction with ID {id} not found." });
            }
            return Ok(transaction);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPost]
    public IActionResult CreateTransaction([FromBody] PostTransactionDto transaction)
    {
        if (transaction == null)
        {
            return BadRequest(new { Message = "Transaction data is required." });
        }
        
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new { Message = "User ID is required." });
            }
            var createdTransaction = service.CreateTransactionAsync(transaction, Guid.Parse(userId)).Result;
            return CreatedAtAction(nameof(GetTransaction), new { id = createdTransaction.Id }, createdTransaction);
        }
        catch (DbUpdateException ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
    
    [HttpPut("Update/{id}")]
    public async Task<IActionResult> UpdateTransaction(Guid id, [FromBody] PostTransactionDto transaction)
    {
        if (transaction == null)
        {
            return BadRequest(new { Message = "Transaction data is required." });
        }

        var existingTransaction = service.GetTransactionByIdAsync(id).Result;
        if (existingTransaction == null)
        {
            return NotFound(new { Message = $"Transaction with ID {id} not found." });
        }

        try
        {
            var updatedTransaction = await service.UpdateTransactionAsync(id, transaction);
            return Ok(updatedTransaction);
        }
        catch (DbUpdateException ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
    
    [HttpDelete("{id}")]
    public IActionResult DeleteTransaction(Guid id)
    {
        try
        {
            service.DeleteTransactionAsync(id).Wait();
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
        catch (DbUpdateException ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
    
}