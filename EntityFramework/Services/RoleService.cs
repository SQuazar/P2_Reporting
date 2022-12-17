using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Models;
using Domain.Services;
using Microsoft.EntityFrameworkCore;

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
        await using var ctx = _factory.CreateDbContext();
        return await ctx.Roles.Include(r => r.Accounts)
            .ToListAsync();
    }

    public async Task<Role> Get(int id)
    {
        await using var ctx = _factory.CreateDbContext();
        return await ctx.Roles.FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Role> Create(Role entity)
    {
        await using var ctx = _factory.CreateDbContext();
        var result = await ctx.Roles.AddAsync(entity);
        await ctx.SaveChangesAsync();
        return result.Entity;
    }

    public async Task<Role> Update(int id, Role entity)
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
        var find = await ctx.Roles.FirstOrDefaultAsync(r => r.Id == id);
        if (find == null) return false;
        ctx.Roles.Remove(find);
        return true;
    }
}