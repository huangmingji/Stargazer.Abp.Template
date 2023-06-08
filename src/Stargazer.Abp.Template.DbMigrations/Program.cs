using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace Stargazer.Abp.Template.DbMigrations;

class Program
{
    static async Task Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Volo.Abp", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.File(Path.Combine(Directory.GetCurrentDirectory(), "logs/logs.txt"))
            .WriteTo.Console()
            .CreateLogger();
        
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        await CreateHostBuilder(args).RunConsoleAsync();
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureLogging((context, logging) => logging.ClearProviders())
            .ConfigureServices((hostContext, services) =>
            {
                services.AddHostedService<DbMigratorHostedService>();
            });
}