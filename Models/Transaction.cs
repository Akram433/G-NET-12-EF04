namespace BankManagementSystem.Models;

public enum TransactionType { Deposit, Withdrawal, Transfer, Payment }

public class Transaction
{
    public int Id { get; set; }
    public string TransactionNumber { get; set; } = null!;
    public DateTime TransactionDate { get; set; }
    public decimal Amount { get; set; }
    public TransactionType TransactionType { get; set; }
    public string? Note { get; set; }

    public int AccountId { get; set; }
    public Account Account { get; set; } = null!;
}
