using BankManagementSystem.Data;
using BankManagementSystem.Menus;
using Microsoft.EntityFrameworkCore;

using var db = new AppDbContext();
db.Database.Migrate();
db.SaveChanges();
while (true)
{
    Console.Clear();
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("========================================");
    Console.WriteLine("        National Bank — Management      ");
    Console.WriteLine("========================================");
    Console.ResetColor();
    Console.WriteLine("  1) Add a new Customer");
    Console.WriteLine("  2) Open a new Account for a Customer");
    Console.WriteLine("  3) Update Account Status (Active / Closed)");
    Console.WriteLine("  4) Remove an Account from a Customer");
    Console.WriteLine("  5) List all Customers (with accounts)");
    Console.WriteLine("  0) Exit");
    Console.WriteLine("----------------------------------------");
    Console.Write("\n  Enter choice: ");

    string? input = Console.ReadLine()?.Trim();

    if (!int.TryParse(input, out int choice))
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("  Invalid input. Please enter a number from the menu.");
        Console.ResetColor();
        Pause();
        continue;
    }

    if (choice == 0) break;

    try
    {
        switch (choice)
        {
            case 1: MenuOperations.AddCustomer(db);             break;
            case 2: MenuOperations.OpenAccount(db);             break;
            case 3: MenuOperations.UpdateAccountStatus(db);     break;
            case 4: MenuOperations.RemoveAccountFromCustomer(db); break;
            case 5: MenuOperations.ListCustomers(db);           break;
            default:
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("  Invalid choice. Please select 0–5.");
                Console.ResetColor();
                break;
        }
    }
    catch (DbUpdateException ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\n  Database error: {ex.InnerException?.Message ?? ex.Message}");
        Console.ResetColor();
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\n  Unexpected error: {ex.Message}");
        Console.ResetColor();
    }

    Pause();
}

Console.WriteLine("\nGoodbye! Connection closed.");

static void Pause()
{
    Console.WriteLine("\nPress any key to return to the menu...");
    Console.ReadKey(intercept: true);
}
