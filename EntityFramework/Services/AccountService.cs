using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;
using Domain.Services;
using Microsoft.EntityFrameworkCore;

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
        await using var ctx = _factory.CreateDbContext();
        return await ctx.Accounts
            .Include(a => a.Roles)
            .Include(a => a.Reports)
            .ToListAsync();
    }

    public async Task<Account> Get(int id)
    {
        await using var ctx = _factory.CreateDbContext();
        return await ctx.Accounts
            .Include(a => a.Roles)
            .Include(a => a.Reports)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Account> Create(Account entity)
    {
        await using var ctx = _factory.CreateDbContext();
        ctx.Roles.AttachRange(entity.Roles);
        var result = await ctx.Accounts.AddAsync(entity);
        await ctx.SaveChangesAsync();
        return result.Entity;
    }

    public async Task<Account> Update(int id, Account entity)
    {
        await using var ctx = _factory.CreateDbContext();
        entity.Id = id;
        ctx.Entry(entity).State = EntityState.Modified;
        await ctx.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> Delete(int id)
    {
        await using var ctx = _factory.CreateDbContext();
        var account = await ctx.Accounts.FirstOrDefaultAsync(a => a.Id == id);
        ctx.Accounts.Remove(account);
        await ctx.SaveChangesAsync();
        return true;
    }

    public async Task<Account> GetByUsername(string username)
    {
        await using var ctx = _factory.CreateDbContext();
        return await ctx.Accounts
            .Include(a => a.Roles)
            .Include(a => a.Reports)
            .FirstOrDefaultAsync(a => a.Username == username);
    }
}