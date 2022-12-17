using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;
using Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Services;

public class AccountRoleService : IAccountRoleService
{
    private readonly ReportingDbContextFactory _factory;
    private readonly IAccountService _accountService;

    public AccountRoleService(ReportingDbContextFactory factory, IAccountService accountService)
    {
        _factory = factory;
        _accountService = accountService;
    }

    public async Task<IEnumerable<AccountRole>> GetAll()
    {
        await using var ctx = _factory.CreateDbContext();
        return await ctx.AccountRoles
            .Include(ar => ar.Account)
            .Include(ar => ar.Role)
            .ToListAsync();
    }

    public Task<AccountRole> Get(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<AccountRole> Create(AccountRole entity)
    {
        await using var ctx = _factory.CreateDbContext();
        var result = await ctx.AddAsync(entity);
        await ctx.SaveChangesAsync();
        return result.Entity;
    }

    public Task<AccountRole> Update(int id, AccountRole entity)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Delete(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<Account> AddRole(Account account, int roleId)
    {
        return await AddRoles(account, new[] { roleId });
    }

    public async Task<Account> AddRoles(Account account, IEnumerable<int> rolesId)
    {
        await using var ctx = _factory.CreateDbContext();
        await using (var transaction = await ctx.Database.BeginTransactionAsync())
        {
            try
            {
                var roles = await ctx.AccountRoles
                    .Where(ar => ar.AccountId == account.Id)
                    .Select(ar => ar.RoleId)
                    .ToListAsync();
                rolesId = rolesId.Where(id => !roles.Contains(id));
                foreach (var roleId in rolesId)
                {
                    await ctx.AccountRoles.AddAsync(new AccountRole { AccountId = account.Id, RoleId = roleId });
                }

                await ctx.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }

        return await _accountService.Get(account.Id);
    }

    public async Task<Account> RemoveRole(Account account, int roleId)
    {
        return await RemoveRoles(account, new[] { roleId });
    }

    public async Task<Account> RemoveRoles(Account account, IEnumerable<int> rolesId)
    {
        await using var ctx = _factory.CreateDbContext();
        await using (var transaction = await ctx.Database.BeginTransactionAsync())
        {
            try
            {
                var roles = await ctx.AccountRoles
                    .Where(ar => ar.AccountId == account.Id)
                    .Where(ar => rolesId.Any(id => ar.RoleId == id))
                    .ToListAsync();
                ctx.AccountRoles.RemoveRange(roles);
                await ctx.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }

        return await _accountService.Get(account.Id);
    }

    public async Task<Account> ClearRoles(Account account)
    {
        await using var ctx = _factory.CreateDbContext();
        var roles = await ctx.AccountRoles.Where(ar => ar.AccountId == account.Id).ToListAsync();
        ctx.AccountRoles.RemoveRange(roles);
        await ctx.SaveChangesAsync();
        return await _accountService.Get(account.Id);
    }
}