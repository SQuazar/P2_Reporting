using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace WPF.HostBuilders;

public static class AddConfigurationHostBuilderExtensions
{
    public static IHostBuilder AddConfiguration(this IHostBuilder host)
    {
        host.ConfigureAppConfiguration(c =>
        {
            c.AddJsonFile("appsettings.json");
            c.AddEnvironmentVariables();
        });
        return host;
    }

    public static string GetDatabaseType(this IConfiguration configuration)
    {
        return configuration.GetSection("database").Value;
    }

    public static string GetSqliteConnectionString(this IConfiguration configuration)
    {
        return configuration.GetSection("sql").GetSection("connection_string").Value;
    }

    public static string GetMySqlConnectionString(this IConfiguration configuration)
    {
        var section = configuration.GetSection("mysql");
        return $"server={section.GetSection("server").Value};" +
               $"user={section.GetSection("user").Value};password={section.GetSection("password").Value};" +
               $"database={section.GetSection("database").Value}";
    }

    public static string GetMySqlVersion(this IConfiguration configuration)
    {
        return configuration.GetSection("mysql").GetSection("version").Value;
    }

}