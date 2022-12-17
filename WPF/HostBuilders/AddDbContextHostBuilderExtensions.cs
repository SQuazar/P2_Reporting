using System;
using EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace WPF.HostBuilders;

public static class AddDbContextHostBuilderExtensions
{
    public static IHostBuilder AddDbContext(this IHostBuilder host)
    {
        host.ConfigureServices((ctx, services) =>
        {
            void Configure(DbContextOptionsBuilder builder)
            {
                var config = ctx.Configuration;
                if (config.GetDatabaseType().ToLower().Equals("mysql"))
                {
                    builder.UseMySql(config.GetMySqlConnectionString(), new MySqlServerVersion(config.GetMySqlVersion()));
                }

                if (config.GetDatabaseType().ToLower().Equals("sql"))
                {
                    builder.UseSqlite(config.GetSqliteConnectionString());
                }
            }

            services.AddDbContext<ReportingDbContext>((Action<DbContextOptionsBuilder>)Configure);
            services.AddSingleton<ReportingDbContextFactory>(new ReportingDbContextFactory(Configure));
        });
        return host;
    }
}