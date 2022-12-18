using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Models;
using Domain.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace EntityFramework.Services;

public class RoleService : IRoleService
{
    private readonly ReportingDbContextFactory _factory;

    public RoleService(ReportingDbContextFactory factory)
    {
        _factory = factory;
    }

    public async Task<IEnumerable<Role>> GetAll()
    {
        Log.Debug("Getting all roles from database");
        await using var ctx = _factory.CreateDbContext();
        return await ctx.Roles.Include(r => r.Accounts)
            .ToListAsync();
    }

    public async Task<Role> Get(int id)
    {
        Log.Debug("Getting role ({@roleId}) from database", id);
        await using var ctx = _factory.CreateDbContext();
        return await ctx.Roles.FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Role> Create(Role entity)
    {
        Log.Debug("Creating new role entity");
        await using var ctx = _factory.CreateDbContext();
        var result = await ctx.Roles.AddAsync(entity);
        await ctx.SaveChangesAsync();
        Log.Debug("Role ({@roleId}) successfully created", result.Entity.Id);
        return result.Entity;
    }

    public async Task<Role> Update(int id, Role entity)
    {
        Log.Debug("Updating role ({@roleId})", id);
        await using var ctx = _factory.CreateDbContext();
        entity.Id = id;
        ctx.Entry(entity).State = EntityState.Modified;
        await ctx.SaveChangesAsync();
        Log.Debug("Role ({@roleId}) successfully updated", id);
        return entity;
    }

    public async Task<bool> Delete(int id)
    {
        Log.Debug("Deleting role ({@roleId})", id);
        await using var ctx = _factory.CreateDbContext();
        var find = await ctx.Roles.FirstOrDefaultAsync(r => r.Id == id);
        if (find == null) return false;
        ctx.Roles.Remove(find);
        await ctx.SaveChangesAsync();
        Log.Debug("Role ({@roleId}) successfully deleted", id);
        return true;
    }
}