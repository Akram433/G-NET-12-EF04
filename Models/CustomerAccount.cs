namespace BankManagementSystem.Models;

public enum OwnershipType { Primary, CoHolder }
public enum AccountStatus { Active, Closed }

public class CustomerAccount
{
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public int AccountId { get; set; }
    public Account Account { get; set; } = null!;

    public OwnershipType OwnershipType { get; set; }
    public DateTime OwnershipStartDate { get; set; }
    public AccountStatus AccountStatus { get; set; }
}
