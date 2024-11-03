using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using StargazerGateway.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.Configure<CustomRateLimitOptions>(builder.Configuration.GetSection(CustomRateLimitOptions.CustomRateLimit));
var rateLimitOptions = new CustomRateLimitOptions();
builder.Configuration.GetSection(CustomRateLimitOptions.CustomRateLimit).Bind(rateLimitOptions);

#region CORS
string DefaultCorsPolicyName = "DefaultCors";
builder.Services.AddCors(options =>
{
    options.AddPolicy(DefaultCorsPolicyName, policy =>
    {
        string[] origins = new[] { "*" };
        string corsOrigins = builder.Configuration["App:CorsOrigins"] ?? "";
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
string DefaultCachePolicyName = "DefaultCache";
builder.Services.AddOutputCache(options =>
{
    string expire = builder.Configuration["App:CacheExpire"] ?? "20";
    int expireSeconds = int.Parse(expire);
    options.AddPolicy("NoCache", builder => builder.NoCache());
    options.AddPolicy(DefaultCachePolicyName, builder => builder.Expire(TimeSpan.FromSeconds(20)));
    options.AddPolicy("CustomCache", builder => builder.Expire(TimeSpan.FromSeconds(expireSeconds)));
});

#endregion

#region rate limiter
string DefaultRateLimiterPolicyName = "DefaultRateLimiter";
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter(DefaultRateLimiterPolicyName, opt =>
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
builder.Services.AddRequestTimeouts(options =>
{
    options.AddPolicy("CustomPolicy", TimeSpan.FromSeconds(30));
});
#endregion

var app = builder.Build();
app.UseHsts();
app.UseCors();
app.UseOutputCache();
app.UseRateLimiter();
app.UseRequestTimeouts();
app.MapReverseProxy();

app.Run();
