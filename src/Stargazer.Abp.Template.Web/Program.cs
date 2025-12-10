using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Serilog;
using Serilog.Events;
using Stargazer.Abp.Template.Web;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.Host.UseAutofac().UseSerilog();

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", true)
    .Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
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

    app.MapDefaultEndpoints();
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
