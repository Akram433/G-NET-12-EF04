namespace BankManagementSystem.Models;

public class Manager
{
    public int Id { get; set; }
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public DateTime HireDate { get; set; }

    public int BranchId { get; set; }
    public Branch Branch { get; set; } = null!;
}
