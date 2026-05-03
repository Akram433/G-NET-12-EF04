namespace BankManagementSystem.Models;

public enum AccountType { Savings, Current, Business }

public class Account
{
    public int Id { get; set; }
    public string AccountNumber { get; set; } = null!;   
    public AccountType AccountType { get; set; }
    public DateTime OpeningDate { get; set; }
    public decimal CurrentBalance { get; set; }
    public int BranchId { get; set; }
    public Branch Branch { get; set; } = null!;

    public ICollection<CustomerAccount> CustomerAccounts { get; set; } = new List<CustomerAccount>();
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
