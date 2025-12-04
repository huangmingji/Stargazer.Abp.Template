using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Events;
using Stargazer.Abp.Template.Host;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

builder.Host.UseAutofac().UseSerilog();

Log.Logger = new LoggerConfiguration()
        // .ReadFrom.AppSettings()
        // .CreateLogger();
    .MinimumLevel.Debug()
    .Enrich.FromLogContext()
    .WriteTo.Seq("http://localhost:5341", LogEventLevel.Information, bufferBaseFilename: "api")
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
    builder.Services.AddOpenApi(options =>
    {
        options.ShouldInclude = (_) => true;
        options.AddDocumentTransformer((document, context, cancellationToken) =>
        {
            document.Info = new()
            {
                Title = "Stargazer API",
                Version = "v1",
                Description = "Stargazer API"
            };
            return Task.CompletedTask;
        });
        options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
    });
    var app = builder.Build();
    app.InitializeApplication();
    if(!app.Environment.IsProduction())
    {
        app.MapOpenApi();
        app.MapScalarApiReference(options =>
            options.WithTitle("Stargazer EShop API")
                .AddPreferredSecuritySchemes(JwtBearerDefaults.AuthenticationScheme)
                .AddHttpAuthentication(JwtBearerDefaults.AuthenticationScheme, auth => { auth.Token = ""; })
        );
    }
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

internal sealed class BearerSecuritySchemeTransformer(IAuthenticationSchemeProvider authenticationSchemeProvider) : IOpenApiDocumentTransformer
{
    public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        var authenticationSchemes = await authenticationSchemeProvider.GetAllSchemesAsync();
        if (authenticationSchemes.Any(authScheme => authScheme.Name == "Bearer"))
        {
            var requirements = new Dictionary<string, IOpenApiSecurityScheme>
            {
                ["Bearer"] = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer", // "bearer" refers to the header name here
                    In = ParameterLocation.Header,
                    BearerFormat = "Json Web Token"
                }
            };
            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes = requirements;
        }
    }
}