using BankManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace BankManagementSystem.Data;

public class AppDbContext : DbContext
{
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<Manager> Managers => Set<Manager>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<CustomerAccount> CustomerAccounts => Set<CustomerAccount>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(
            "Server=AK\\SQLSERVER;Database=NationalBankDb;Trusted_Connection=True;TrustServerCertificate=True;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Branch>(e =>
        {
            e.HasKey(b => b.Id);
            e.Property(b => b.Code).IsRequired().HasMaxLength(20);
            e.HasIndex(b => b.Code).IsUnique();
            e.Property(b => b.Name).IsRequired().HasMaxLength(100);
            e.Property(b => b.Address).IsRequired().HasMaxLength(200);
            e.Property(b => b.PhoneNumber).IsRequired().HasMaxLength(20);
        });

        modelBuilder.Entity<Manager>(e =>
        {
            e.HasKey(m => m.Id);
            e.Property(m => m.FullName).IsRequired().HasMaxLength(100);
            e.Property(m => m.Email).IsRequired().HasMaxLength(150);
            e.Property(m => m.PhoneNumber).IsRequired().HasMaxLength(20);

            e.HasOne(m => m.Branch)
             .WithOne(b => b.Manager)
             .HasForeignKey<Manager>(m => m.BranchId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Customer>(e =>
        {
            e.HasKey(c => c.Id);
            e.Property(c => c.FullName).IsRequired().HasMaxLength(100);
            e.Property(c => c.NationalId).IsRequired().HasMaxLength(20);
            e.HasIndex(c => c.NationalId).IsUnique();
            e.Property(c => c.Email).IsRequired().HasMaxLength(150);
            e.Property(c => c.PhoneNumber).IsRequired().HasMaxLength(20);
            e.Property(c => c.Address).IsRequired().HasMaxLength(200);
            e.Property(c => c.CustomerType)
             .HasConversion<string>().IsRequired();
        });

        modelBuilder.Entity<Account>(e =>
        {
            e.HasKey(a => a.Id);
            e.Property(a => a.AccountNumber).IsRequired().HasMaxLength(50);
            e.HasIndex(a => a.AccountNumber).IsUnique();
            e.Property(a => a.AccountType).HasConversion<string>().IsRequired();
            e.Property(a => a.CurrentBalance).HasColumnType("decimal(18,2)");

            e.HasOne(a => a.Branch)
             .WithMany(b => b.Accounts)
             .HasForeignKey(a => a.BranchId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<CustomerAccount>(e =>
        {
            e.HasKey(ca => new { ca.CustomerId, ca.AccountId });

            e.Property(ca => ca.OwnershipType)
             .HasConversion<string>().IsRequired();
            e.Property(ca => ca.AccountStatus)
             .HasConversion<string>().IsRequired();

            e.HasOne(ca => ca.Customer)
             .WithMany(c => c.CustomerAccounts)
             .HasForeignKey(ca => ca.CustomerId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(ca => ca.Account)
             .WithMany(a => a.CustomerAccounts)
             .HasForeignKey(ca => ca.AccountId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Transaction>(e =>
        {
            e.HasKey(t => t.Id);
            e.Property(t => t.TransactionNumber).IsRequired().HasMaxLength(50);
            e.HasIndex(t => t.TransactionNumber).IsUnique();
            e.Property(t => t.Amount).HasColumnType("decimal(18,2)");
            e.Property(t => t.TransactionType).HasConversion<string>().IsRequired();
            e.Property(t => t.Note).HasMaxLength(300);

            e.HasOne(t => t.Account)
             .WithMany(a => a.Transactions)
             .HasForeignKey(t => t.AccountId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Branch>().HasData(
            new Branch { Id = 1, Code = "CAI-01", Name = "Cairo Main Branch",
                         Address = "12 Tahrir Square, Cairo", PhoneNumber = "02-23456789" },
            new Branch { Id = 2, Code = "ALX-01", Name = "Alexandria Branch",
                         Address = "5 Corniche Road, Alexandria", PhoneNumber = "03-34567890" },
            new Branch { Id = 3, Code = "GIZ-01", Name = "Giza Branch",
                         Address = "88 Haram Street, Giza", PhoneNumber = "02-98765432" }
        );

        modelBuilder.Entity<Manager>().HasData(
            new Manager { Id = 1, FullName = "Ahmed Hassan",  Email = "ahmed.hassan@nationalbank.com",
                          PhoneNumber = "0111-1111111", HireDate = new DateTime(2015, 3, 10), BranchId = 1 },
            new Manager { Id = 2, FullName = "Sara Mohamed",  Email = "sara.mohamed@nationalbank.com",
                          PhoneNumber = "0122-2222222", HireDate = new DateTime(2018, 7, 1),  BranchId = 2 },
            new Manager { Id = 3, FullName = "Khaled Nasser", Email = "khaled.nasser@nationalbank.com",
                          PhoneNumber = "0133-3333333", HireDate = new DateTime(2020, 1, 15), BranchId = 3 }
        );

        modelBuilder.Entity<Customer>().HasData(
            new Customer { Id = 1, FullName = "Ahmed Ali", NationalId = "29901010123456",
                           DateOfBirth = new DateTime(1999, 1, 1), Email = "ahmed.ali@gmail.com",
                           PhoneNumber = "0100-1234567", Address = "10 Nile St, Cairo",
                           CustomerType = CustomerType.Individual },
            new Customer { Id = 2, FullName = "Nile Trading LLC", NationalId = "TAX-987654321",
                           DateOfBirth = new DateTime(2010, 5, 15), Email = "info@niletrading.com",
                           PhoneNumber = "0100-9999999", Address = "55 Port Said St, Alexandria",
                           CustomerType = CustomerType.Business }
        );

        modelBuilder.Entity<Account>().HasData(
            new Account { Id = 1, AccountNumber = "2001-CUR", AccountType = AccountType.Current,
                          OpeningDate = new DateTime(2022, 1, 10), CurrentBalance = 15400.00m, BranchId = 1 },
            new Account { Id = 2, AccountNumber = "3000-BUS", AccountType = AccountType.Business,
                          OpeningDate = new DateTime(2022, 3, 20), CurrentBalance = 120000.00m, BranchId = 2 }
        );

        modelBuilder.Entity<CustomerAccount>().HasData(
            new CustomerAccount { CustomerId = 1, AccountId = 1, OwnershipType = OwnershipType.Primary,
                                  OwnershipStartDate = new DateTime(2022, 1, 10), AccountStatus = AccountStatus.Active },
            new CustomerAccount { CustomerId = 2, AccountId = 2, OwnershipType = OwnershipType.Primary,
                                  OwnershipStartDate = new DateTime(2022, 3, 20), AccountStatus = AccountStatus.Active }
        );
    }
}
