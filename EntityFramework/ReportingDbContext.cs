using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework;

public sealed class ReportingDbContext : DbContext
{
    public DbSet<Account> Accounts { get; set; } = null!;
    public DbSet<Role> Roles { get; set; } = null!;
    public DbSet<AccountRole> AccountRoles { get; set; } = null!;
    public DbSet<Report> Reports { get; set; } = null!;

    public ReportingDbContext(DbContextOptions options) : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region Accounts and roles

        modelBuilder.Entity<Account>()
            .HasMany(a => a.Roles)
            .WithMany(r => r.Accounts)
            .UsingEntity<AccountRole>();
        modelBuilder.Entity<Role>()
            .HasMany(r => r.Accounts)
            .WithMany(a => a.Roles)
            .UsingEntity<AccountRole>();
        modelBuilder.Entity<AccountRole>()
            .ToTable("account_roles")
            .HasKey(ar => new { ar.AccountId, ar.RoleId });

        Role adminRole;
        Account adminAccount;
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, AccessLevel = 0, Name = "Пользователь" },
            new Role { Id = 2, AccessLevel = 1, Name = "Бухгалтер" },
            adminRole = new Role { Id = 3, AccessLevel = 2, Name = "Администратор" }
        );
        modelBuilder.Entity<Account>().HasData(adminAccount = new Account
        {
            Id = 1, Username = "root", Firstname = "root", Surname = "root",
            PasswordHash = "ACU0lO4bCsZQerLc0lPcfCdxiE+yYbSKUPCD1+5I8cj/PNCs06Ku90OhC9JidNs/1Q==",
        });
        modelBuilder.Entity<AccountRole>()
            .ToTable("account_role")
            .HasData(new AccountRole { AccountId = adminAccount.Id, RoleId = adminRole.Id });

        #endregion

        #region Reports

        modelBuilder.Entity<Report>().Property(r => r.ReportState)
            .HasConversion(
                state => state.Name,
                s => Report.State.ValueOf(s)
            );
        modelBuilder.Entity<Report>()
            .HasOne(r => r.Agent)
            .WithMany(a => a.AttachedReports);

        #endregion

        base.OnModelCreating(modelBuilder);
    }
}