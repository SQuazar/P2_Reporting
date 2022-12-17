using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;
using Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Services;

public class ReportService : IReportService
{
    private readonly ReportingDbContextFactory _factory;

    public ReportService(ReportingDbContextFactory factory)
    {
        _factory = factory;
    }

    public async Task<IEnumerable<Report>> GetAll()
    {
        await using var ctx = _factory.CreateDbContext();
        return await ctx.Reports
            .Include(r => r.Sender)
            .Include(r => r.Agent)
            .ToListAsync();
    }

    public async Task<Report> Get(int id)
    {
        await using var ctx = _factory.CreateDbContext();
        return await ctx.Reports.FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Report> Create(Report entity)
    {
        await using var ctx = _factory.CreateDbContext();
        var result = await ctx.AddAsync(entity);
        await ctx.SaveChangesAsync();
        return result.Entity;
    }

    public async Task<Report> Update(int id, Report entity)
    {
        await using var ctx = _factory.CreateDbContext();
        entity.Id = id;
        ctx.Entry(entity).State = EntityState.Modified;
        if (entity.Agent != null)
            ctx.Entry(entity.Agent).State = EntityState.Modified;
        await ctx.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> Delete(int id)
    {
        await using var ctx = _factory.CreateDbContext();
        var find = await ctx.Reports.FirstOrDefaultAsync(r => r.Id == id);
        if (find == null) return false;
        ctx.Reports.Remove(find);
        await ctx.SaveChangesAsync();
        return true;
    }

    public async Task<Report> Send(Account sender, string title, string description = null)
    {
        return await Create(new Report { SenderId = sender.Id, Title = title, Description = description });
    }

    public async Task<IEnumerable<Report>> GetPage(int page, int size)
    {
        page = page < 0 ? 1 : page - 1;
        await using var ctx = _factory.CreateDbContext();
        return await ctx.Reports.OrderByDescending(r => r.ReportDate)
            .Include(r => r.Sender)
            .Include(r => r.Agent)
            .Skip(page * size)
            .Take(size).ToListAsync();
    }

    public async Task<int> GetPagesCount(int size)
    {
        await using var ctx = _factory.CreateDbContext();
        return await ctx.Reports.CountAsync() / size + 1;
    }
}