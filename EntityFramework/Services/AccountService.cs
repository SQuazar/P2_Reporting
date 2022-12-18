using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Models;
using Domain.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace EntityFramework.Services;

public class AccountService : IAccountService
{
    private readonly ReportingDbContextFactory _factory;

    public AccountService(ReportingDbContextFactory factory)
    {
        _factory = factory;
    }
    
    public async Task<IEnumerable<Account>> GetAll()
    {
        Log.Debug("Getting all accounts from database");
        await using var ctx = _factory.CreateDbContext();
        return await ctx.Accounts
            .Include(a => a.Roles)
            .Include(a => a.Reports)
            .ToListAsync();
    }

    public async Task<Account> Get(int id)
    {
        Log.Debug("Getting account ({@accountId}) from database", id);
        await using var ctx = _factory.CreateDbContext();
        return await ctx.Accounts
            .Include(a => a.Roles)
            .Include(a => a.Reports)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Account> Create(Account entity)
    {
        Log.Debug("Creating new account entity");
        await using var ctx = _factory.CreateDbContext();
        ctx.Roles.AttachRange(entity.Roles);
        var result = await ctx.Accounts.AddAsync(entity);
        await ctx.SaveChangesAsync();
        Log.Debug("Account ({@accountId}) created successfully", result.Entity.Id);
        return result.Entity;
    }

    public async Task<Account> Update(int id, Account entity)
    {
        Log.Debug("Updating account ({@accountId})", id);
        await using var ctx = _factory.CreateDbContext();
        entity.Id = id;
        ctx.Entry(entity).State = EntityState.Modified;
        await ctx.SaveChangesAsync();
        Log.Debug("Account ({@accountId}) is updated successfully", id);
        return entity;
    }

    public async Task<bool> Delete(int id)
    {
        Log.Debug("Deleting account ({@accountId})", id);
        await using var ctx = _factory.CreateDbContext();
        var account = await ctx.Accounts.FirstOrDefaultAsync(a => a.Id == id);
        ctx.Accounts.Remove(account);
        await ctx.SaveChangesAsync();
        Log.Debug("Account ({@accountId}) is deleted successfully", id);
        return true;
    }

    public async Task<Account> GetByUsername(string username)
    {
        Log.Debug("Getting account by username ({@username}) from database", username);
        await using var ctx = _factory.CreateDbContext();
        return await ctx.Accounts
            .Include(a => a.Roles)
            .Include(a => a.Reports)
            .FirstOrDefaultAsync(a => a.Username == username);
    }
}