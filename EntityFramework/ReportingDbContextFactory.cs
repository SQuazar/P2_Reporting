using System;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework;

public class ReportingDbContextFactory
{
    private readonly Action<DbContextOptionsBuilder> _configureDbContext;

    public ReportingDbContextFactory(Action<DbContextOptionsBuilder> configureDbContext)
    {
        _configureDbContext = configureDbContext;
    }

    public ReportingDbContext CreateDbContext()
    {
        var optionsBuilder = new DbContextOptionsBuilder<ReportingDbContext>();
        _configureDbContext(optionsBuilder);
        return new ReportingDbContext(optionsBuilder.Options);
    }
}