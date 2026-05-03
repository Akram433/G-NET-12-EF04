using BankManagementSystem.Data;
using BankManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace BankManagementSystem.Menus;

public static class MenuOperations
{
    public static void AddCustomer(AppDbContext db)
    {
        Console.WriteLine("\n--- Add New Customer ---");

        string fullName = Prompt("Full Name     ");
        string nationalId = Prompt("National ID   ");
        DateTime dob = PromptDate("Date of Birth : (yyyy-MM-dd) ");
        string email = Prompt("Email         ");
        string phone = Prompt("Phone         ");
        string address = Prompt("Address       ");

        Console.WriteLine("Customer Type:");
        Console.WriteLine("     1) Individual");
        Console.WriteLine("     2) Business");
        int typeChoice = PromptInt("  Choice", 1, 2);
        var cType = typeChoice == 1 ? CustomerType.Individual : CustomerType.Business;

        var customer = new Customer
        {
            FullName     = fullName,
            NationalId   = nationalId,
            DateOfBirth  = dob,
            Email        = email,
            PhoneNumber  = phone,
            Address      = address,
            CustomerType = cType
        };

        db.Customers.Add(customer);
        db.SaveChanges();

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\nCustomer created successfully. CustomerId = {customer.Id}");
        Console.ResetColor();
    }

    public static void OpenAccount(AppDbContext db)
    {
        Console.WriteLine("\n--- Open New Account ---");

        string accountNumber = Prompt("Account Number");

        Console.WriteLine("Account Type:");
        Console.WriteLine("     1) Savings");
        Console.WriteLine("     2) Current");
        Console.WriteLine("     3) Business");
        int atChoice = PromptInt("  Choice", 1, 3);
        var accountType = atChoice switch { 1 => AccountType.Savings, 2 => AccountType.Current, _ => AccountType.Business };

        string branchCode = Prompt("Branch Code   ");
        int customerId = PromptInt("Customer Id   ");

        Console.WriteLine("Ownership Role:");
        Console.WriteLine("     1) Primary");
        Console.WriteLine("     2) CoHolder");
        int roleChoice = PromptInt("  Choice", 1, 2);
        var ownershipType = roleChoice == 1 ? OwnershipType.Primary : OwnershipType.CoHolder;

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"Validating branch '{branchCode}' and customer #{customerId}...");
        Console.ResetColor();

        var branch = db.Branches.FirstOrDefault(b => b.Code == branchCode);
        if (branch == null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: Branch '{branchCode}' does not exist.");
            Console.ResetColor();
            return;
        }

        var customer = db.Customers.FirstOrDefault(c => c.Id == customerId);
        if (customer == null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: Customer #{customerId} does not exist.");
            Console.ResetColor();
            return;
        }

        if (db.Accounts.Any(a => a.AccountNumber == accountNumber))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: Account number '{accountNumber}' already exists.");
            Console.ResetColor();
            return;
        }

        var account = new Account
        {
            AccountNumber  = accountNumber,
            AccountType    = accountType,
            OpeningDate    = DateTime.Today,
            CurrentBalance = 0,
            BranchId       = branch.Id
        };
        db.Accounts.Add(account);
        db.SaveChanges();

        var ca = new CustomerAccount
        {
            CustomerId         = customerId,
            AccountId          = account.Id,
            OwnershipType      = ownershipType,
            OwnershipStartDate = DateTime.Today,
            AccountStatus      = AccountStatus.Active
        };
        db.CustomerAccounts.Add(ca);
        db.SaveChanges();

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Account '{accountNumber}' created and linked to customer {customerId} as {ownershipType} owner.");
        Console.ResetColor();
    }

    public static void UpdateAccountStatus(AppDbContext db)
    {
        Console.WriteLine("\n--- Update Account Status ---");

        string accountNumber = Prompt("Account Number");
        int customerId = PromptInt("Customer Id   ");

        var ca = db.CustomerAccounts
                   .Include(x => x.Account)
                   .FirstOrDefault(x => x.Account.AccountNumber == accountNumber
                                     && x.CustomerId == customerId);

        if (ca == null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Error: No matching account/customer link found.");
            Console.ResetColor();
            return;
        }

        Console.WriteLine("New Status:");
        Console.WriteLine("     1) Active");
        Console.WriteLine("     2) Closed");
        int choice = PromptInt("  Choice", 1, 2);
        ca.AccountStatus = choice == 1 ? AccountStatus.Active : AccountStatus.Closed;
        db.SaveChanges();

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Status updated to {ca.AccountStatus}.");
        Console.ResetColor();
    }

    public static void RemoveAccountFromCustomer(AppDbContext db)
    {
        Console.WriteLine("\n--- Remove Account From Customer ---");

        string accountNumber = Prompt("Account Number");
        int customerId = PromptInt("Customer Id   ");

        var ca = db.CustomerAccounts
                   .Include(x => x.Account)
                   .FirstOrDefault(x => x.Account.AccountNumber == accountNumber
                                     && x.CustomerId == customerId);

        if (ca == null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Error: No matching account/customer link found.");
            Console.ResetColor();
            return;
        }

        int accountId = ca.AccountId;
        db.CustomerAccounts.Remove(ca);
        db.SaveChanges();

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("  Ownership link deleted.");

        bool hasMoreOwners = db.CustomerAccounts.Any(x => x.AccountId == accountId);
        if (!hasMoreOwners)
        {
            var account = db.Accounts.Find(accountId);
            if (account != null)
            {
                db.Accounts.Remove(account);
                db.SaveChanges();
                Console.WriteLine($"      That was the last owner — account '{accountNumber}' was also removed.");
            }
        }
        Console.ResetColor();
    }

    public static void ListCustomers(AppDbContext db)
    {
        Console.WriteLine("\n--- All Customers ---\n");

        var customers = db.Customers
            .Include(c => c.CustomerAccounts)
                .ThenInclude(ca => ca.Account)
                    .ThenInclude(a => a.Branch)
            .OrderBy(c => c.Id)
            .ToList();

        foreach (var c in customers)
        {
            Console.WriteLine($"  #{c.Id} {c.FullName} ({c.CustomerType})");
            if (!c.CustomerAccounts.Any())
            {
                Console.WriteLine("       (no accounts)");
            }
            else
            {
                foreach (var ca in c.CustomerAccounts)
                {
                    var a = ca.Account;
                    Console.WriteLine(
                        $"       {a.AccountNumber,-12} {a.AccountType,-10} Balance: {a.CurrentBalance,12:F2}  " +
                        $"{ca.OwnershipType,-10} {ca.AccountStatus,-8} @ {a.Branch.Name}");
                }
            }
        }
    }

    private static string Prompt(string label)
    {
        while (true)
        {
            Console.Write($"{label} : ");
            string? value = Console.ReadLine()?.Trim();
            if (!string.IsNullOrWhiteSpace(value)) return value;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("  Input cannot be empty. Please try again.");
            Console.ResetColor();
        }
    }

    private static int PromptInt(string label, int min = int.MinValue, int max = int.MaxValue)
    {
        while (true)
        {
            Console.Write($"{label} : ");
            string? raw = Console.ReadLine()?.Trim();
            if (int.TryParse(raw, out int value) && value >= min && value <= max)
                return value;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  Invalid input. Please enter a number{(min != int.MinValue ? $" between {min} and {max}" : "")}.");
            Console.ResetColor();
        }
    }

    private static DateTime PromptDate(string label)
    {
        while (true)
        {
            Console.Write(label);
            string? raw = Console.ReadLine()?.Trim();
            if (DateTime.TryParseExact(raw, "yyyy-MM-dd",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out DateTime dt))
                return dt;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("  Invalid date. Please use yyyy-MM-dd format.");
            Console.ResetColor();
        }
    }
}
