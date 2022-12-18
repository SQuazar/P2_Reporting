using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;
using Domain.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;

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
        Log.Debug("Getting all accounts roles from database");
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
        Log.Debug("Creating new account role entity");
        await using var ctx = _factory.CreateDbContext();
        var result = await ctx.AddAsync(entity);
        await ctx.SaveChangesAsync();
        Log.Debug("Account role ({@accountId}, {@roleId}) is successful created",
            result.Entity.AccountId, result.Entity.RoleId);
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

    [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
    public async Task<Account> AddRoles(Account account, IEnumerable<int> rolesId)
    {
        Log.Debug("Adding roles [{@rolesIds}] to account ({@accountId})",
            string.Join(',', rolesId), account.Id);
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
                Log.Debug("Roles [{@rolesIds}] is successful added to account ({@accountId})",
                    string.Join(',', rolesId), account.Id);
            }
            catch (Exception e)
            {
                Log.Error(e, "Adding roles was throwing an exception");
            }
        }

        return await _accountService.Get(account.Id);
    }

    public async Task<Account> RemoveRole(Account account, int roleId)
    {
        return await RemoveRoles(account, new[] { roleId });
    }

    [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
    public async Task<Account> RemoveRoles(Account account, IEnumerable<int> rolesId)
    {
        Log.Debug("Removing roles [{@rolesIds}] from account ({@accountId})",
            string.Join(',', rolesId), account.Id);
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
                Log.Debug("Roles [{@rolesIds}] is successful removed from account ({@accountId})",
                    string.Join(',', rolesId), account.Id);
            }
            catch (Exception e)
            {
                Log.Error(e, "Removing roles was throwing an exception");
            }
        }

        return await _accountService.Get(account.Id);
    }

    public async Task<Account> ClearRoles(Account account)
    {
        Log.Debug("Removing all roles from account ({@accountId})", account.Id);
        await using var ctx = _factory.CreateDbContext();
        var roles = await ctx.AccountRoles.Where(ar => ar.AccountId == account.Id).ToListAsync();
        ctx.AccountRoles.RemoveRange(roles);
        await ctx.SaveChangesAsync();
        Log.Debug("All roles is successful removed from account ({@accountId})", account.Id);
        return await _accountService.Get(account.Id);
    }
}