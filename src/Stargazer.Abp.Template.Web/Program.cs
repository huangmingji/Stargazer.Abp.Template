using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Serilog;
using Stargazer.Abp.Template.Web;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseAutofac().UseSerilog();

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Async(c =>
        c.File("Logs/log.txt",
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 31,
            rollOnFileSizeLimit: true,
            fileSizeLimitBytes: 31457280,
            buffered: true))
    .WriteTo.Console()
    .CreateLogger();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
try
{
    builder.Services.ReplaceConfiguration(builder.Configuration);
    builder.Services.AddApplication<WebModule>();

    var app = builder.Build();
    app.InitializeApplication();

    app.MapBlazorHub();
    app.MapFallbackToPage("/_Host");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
