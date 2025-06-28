using Expenses.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Expenses.API.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Transaction>().ToTable("Transactions");
        modelBuilder.Entity<User>().ToTable("Users");
    }
}