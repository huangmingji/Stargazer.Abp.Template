using System.Threading.RateLimiting;
using LettuceEncrypt;
using Microsoft.AspNetCore.RateLimiting;
using StargazerGateway;
using StargazerGateway.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();
Log.Logger = new LoggerConfiguration()
#if DEBUG
    .MinimumLevel.Debug()
#else
    .MinimumLevel.Information()
#endif
    .Enrich.FromLogContext()
    .WriteTo.Async(c =>
        c.File("Logs/log.txt",
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 30,
            rollOnFileSizeLimit: true,
            fileSizeLimitBytes: 31457280,
            buffered: true))
    .WriteTo.Console()
    .CreateLogger();
Log.Information("Starting web host.");

try
{
    var env = builder.Environment;
    if (env.IsProduction())
    {
        builder.Services.AddLettuceEncrypt()
            .PersistDataToDirectory(new DirectoryInfo(Path.Combine(env.ContentRootPath, "certs")), "");
        builder.WebHost.ConfigureKestrel(kestrel =>
        {
            kestrel.ListenAnyIP(80);
            kestrel.ListenAnyIP(443,
                portOptions => { portOptions.UseHttps(h => { h.UseLettuceEncrypt(kestrel.ApplicationServices); }); });
        });
    }
    
    var configuration = builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
        .AddEnvironmentVariables().Build();
    
    builder.Services.AddReverseProxy()
        .LoadFromConfig(configuration.GetSection("ReverseProxy"));
    
    builder.Services.Configure<CustomRateLimitOptions>(
        configuration.GetSection(CustomRateLimitOptions.CustomRateLimit));
    var rateLimitOptions = new CustomRateLimitOptions();
    builder.Configuration.GetSection(CustomRateLimitOptions.CustomRateLimit).Bind(rateLimitOptions);

    #region CORS

    string defaultCorsPolicyName = "DefaultCors";
    builder.Services.AddCors(options =>
    {
        options.AddPolicy(defaultCorsPolicyName, policy =>
        {
            string[] origins = new[] {"*"};
            string corsOrigins = configuration["App:CorsOrigins"] ?? "";
            if (!string.IsNullOrWhiteSpace(corsOrigins))
            {
                origins = corsOrigins.Split(",", StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim())
                    .Select(x =>
                    {
                        return x.EndsWith("/", StringComparison.Ordinal) ? x.Substring(0, x.Length - 1) : x;
                    }).ToArray();

                policy = policy.WithOrigins(origins);
            }
            else
            {
                policy = policy.AllowAnyOrigin();
            }

            policy.SetIsOriginAllowedToAllowWildcardSubdomains()
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
    });

    #endregion

    #region cache

    string defaultCachePolicyName = "DefaultCache";
    builder.Services.AddOutputCache(options =>
    {
        string expire = configuration["App:CacheExpire"] ?? "20";
        int expireSeconds = int.Parse(expire);
        options.AddPolicy("NoCache", build => build.NoCache());
        options.AddPolicy(defaultCachePolicyName, build => build.Expire(TimeSpan.FromSeconds(20)));
        options.AddPolicy("CustomCache", build => build.Expire(TimeSpan.FromSeconds(expireSeconds)));
    });

    #endregion

    #region rate limiter

    string defaultRateLimiterPolicyName = "DefaultRateLimiter";
    builder.Services.AddRateLimiter(options =>
    {
        options.AddFixedWindowLimiter(defaultRateLimiterPolicyName, opt =>
        {
            opt.PermitLimit = 4;
            opt.Window = TimeSpan.FromSeconds(12);
            opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            opt.QueueLimit = 2;
        });
        options.AddFixedWindowLimiter("CustomRateLimiter", opt =>
        {
            opt.PermitLimit = rateLimitOptions.PermitLimit;
            opt.Window = TimeSpan.FromSeconds(rateLimitOptions.Window);
            opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            opt.QueueLimit = rateLimitOptions.QueueLimit;
        });
    });

    #endregion

    #region timeouts

    builder.Services.AddRequestTimeouts(options => { options.AddPolicy("CustomPolicy", TimeSpan.FromSeconds(30)); });

    #endregion

    #region HTTPS

    var useHttpsRedirection = configuration["App:UseHttpsRedirection"]?.ToLower() == "true";
    if (env.IsProduction())
    {
        builder.Services.AddHsts(options =>
        {
            options.Preload = true;
            options.IncludeSubDomains = true;
            options.MaxAge = TimeSpan.FromDays(60);
            options.ExcludedHosts.Add("example.com");
            options.ExcludedHosts.Add("www.example.com");
        });
        if (useHttpsRedirection)
        {
            builder.Services.AddHttpsRedirection(options =>
            {
                options.RedirectStatusCode = StatusCodes.Status308PermanentRedirect;
                options.HttpsPort = 443;
            });
        }
    }

    #endregion

    var app = builder.Build();
    if (app.Environment.IsProduction())
    {
        app.UseHsts();
        if (useHttpsRedirection)
        {
            app.UseHttpsRedirection();
        }
    }

    app.UseCors();
    app.UseOutputCache();
    app.UseRateLimiter();
    app.UseRequestTimeouts();
    app.MapReverseProxy(proxyPipeline =>
    {
        proxyPipeline.UseWaf();
        proxyPipeline.UseSessionAffinity();
        proxyPipeline.UseLoadBalancing();
        proxyPipeline.UsePassiveHealthChecks();
    });

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
