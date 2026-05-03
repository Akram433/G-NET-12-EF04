namespace BankManagementSystem.Models;

public class Branch
{
    public int Id { get; set; }
    public string Code { get; set; } = null!;     
    public string Name { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;

    public Manager? Manager { get; set; }
    public ICollection<Account> Accounts { get; set; } = new List<Account>();
}
