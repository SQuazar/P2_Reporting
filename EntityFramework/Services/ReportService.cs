using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;
using Domain.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;

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
        Log.Debug("Getting all reports from database");
        await using var ctx = _factory.CreateDbContext();
        return await ctx.Reports
            .Include(r => r.Sender)
            .Include(r => r.Agent)
            .ToListAsync();
    }

    public async Task<Report> Get(int id)
    {
        Log.Debug("Getting report ({@reportId}) from database", id);
        await using var ctx = _factory.CreateDbContext();
        return await ctx.Reports.FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Report> Create(Report entity)
    {
        Log.Debug("Creating new report entity");
        await using var ctx = _factory.CreateDbContext();
        var result = await ctx.AddAsync(entity);
        await ctx.SaveChangesAsync();
        Log.Debug("Report ({@reportId}) successfully created", result.Entity.Id);
        return result.Entity;
    }

    public async Task<Report> Update(int id, Report entity)
    {
        Log.Debug("Updating report ({@reportId})", id);
        await using var ctx = _factory.CreateDbContext();
        entity.Id = id;
        ctx.Entry(entity).State = EntityState.Modified;
        if (entity.Agent != null)
            ctx.Entry(entity.Agent).State = EntityState.Modified;
        await ctx.SaveChangesAsync();
        Log.Debug("Report ({@reportId}) successfully updated", id);
        return entity;
    }

    public async Task<bool> Delete(int id)
    {
        Log.Debug("Deleting report ({@reportId})", id);
        await using var ctx = _factory.CreateDbContext();
        var find = await ctx.Reports.FirstOrDefaultAsync(r => r.Id == id);
        if (find == null) return false;
        ctx.Reports.Remove(find);
        await ctx.SaveChangesAsync();
        Log.Debug("Report ({@reportId}) successfully deleted", id);
        return true;
    }

    public async Task<Report> Send(Account sender, string title, string description = null)
    {
        return await Create(new Report { SenderId = sender.Id, Title = title, Description = description });
    }

    public async Task<IEnumerable<Report>> GetAllBySender(Account sender)
    {
        Log.Debug("Getting all reports by sender ({@senderId}) from database", sender.Id);
        await using var ctx = _factory.CreateDbContext();
        return await ctx.Reports
            .Include(r => r.Sender)
            .Where(r => r.Sender.Id == sender.Id)
            .Include(r => r.Agent)
            .ToListAsync();
    }

    public async Task<IEnumerable<Report>> GetPage(int page, int size)
    {
        Log.Debug("Getting reports page ({@page}) with size {@size}", page, size);
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