using Scalar.AspNetCore;
using Serilog;
using Stargazer.Abp.Template.Host;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseAutofac().UseSerilog();

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
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

try
{
    // Add services to the container.
    builder.Services.ReplaceConfiguration(builder.Configuration);
    builder.Services.AddApplication<HostModule>();
    builder.Services.AddOpenApi();
    var app = builder.Build();
    app.InitializeApplication();
    if(!app.Environment.IsProduction())
    {
        app.MapOpenApi();
        app.MapScalarApiReference();
    }
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