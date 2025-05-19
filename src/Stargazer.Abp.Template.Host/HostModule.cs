using System.Reflection;
using Lemon.Common.Extend;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using Stargazer.Abp.Authentication.JwtBearer.Application.Authentication;
using Stargazer.Abp.Template.Application;
using Stargazer.Abp.Template.EntityFrameworkCore;
using Stargazer.Abp.Template.HttpApi;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using Volo.Abp.Swashbuckle;

namespace Stargazer.Abp.Template.Host
{
    [DependsOn(
    typeof(EntityFrameworkCoreModule),
    typeof(ApplicationModule),
    typeof(HttpApiModule),
    typeof(AbpAspNetCoreMvcModule),
    typeof(AbpAutofacModule),
    typeof(AbpAspNetCoreSerilogModule),
    typeof(AbpSwashbuckleModule)
    )]
    public class HostModule : AbpModule
    {
        private const string DefaultCorsPolicyName = "Default";
        
        private void ConfigureAuthentication(ServiceConfigurationContext context, IConfiguration configuration)
        {
            context.Services.UseJwtBearerAuthentication(new string[] { });
        }

        private void ConfigureCors(ServiceConfigurationContext context, IConfiguration configuration)
        {
            context.Services.AddCors(options =>
            {
                options.AddPolicy(DefaultCorsPolicyName, builder =>
                {
                    builder
                        .WithOrigins(
                            configuration["App:CorsOrigins"]
                                .Split(",", StringSplitOptions.RemoveEmptyEntries)
                                .Select(o => o.RemovePostFix("/"))
                                .ToArray()
                        )
                        .WithAbpExposedHeaders()
                        .SetIsOriginAllowedToAllowWildcardSubdomains()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });
        }

        private void ConfigureCache(ServiceConfigurationContext context, IConfiguration configuration)
        {
            if (configuration["Redis:IsEnabled"].ToBool())
            {
                context.Services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = configuration["Redis:Configuration"];
                });
                Configure<RedisCacheOptions>(options =>
                {
                    options.Configuration = configuration["Redis:Configuration"];
                });
            }
            else
            {
                context.Services.AddMemoryCache();
            }
        }
        
        private void ConfigureDataProtection(ServiceConfigurationContext context, IConfiguration configuration)
        {
            // 添加数据保护服务，设置统一应用程序名称，
            var dataProtectionBuilder = context.Services.AddDataProtection()
                .SetApplicationName(Assembly.GetExecutingAssembly().FullName ?? "Stargazer.Abp.Template");
            if (configuration["Redis:IsEnabled"].ToBool())
            {
                var redis = ConnectionMultiplexer.Connect(configuration["Redis:Configuration"]);//建立Redis 连接
                // 指定使用Reids存储私钥
                dataProtectionBuilder.PersistKeysToStackExchangeRedis(redis, "DataProtection-Keys");
            }
            else
            {
                dataProtectionBuilder.PersistKeysToFileSystem(new DirectoryInfo(System.AppDomain.CurrentDomain.BaseDirectory));
            }
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();
            var hostingEnvironment = context.Services.GetHostingEnvironment();

            context.Services.AddMvcCore().AddNewtonsoftJson(
                op =>
                {
                    op.SerializerSettings.ContractResolver =
                        new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
                    op.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                    op.SerializerSettings.Converters.Add(new Ext.DateTimeJsonConverter());
                    op.SerializerSettings.Converters.Add(new Ext.LongJsonConverter());
                });

            //swagger
            context.Services.AddAbpSwaggerGen(options =>
                {
                    options.SwaggerDoc("v1", new OpenApiInfo {Title = "Stargazer API", Version = "v1", Description = "Stargazer API"});
                    options.DocInclusionPredicate((docName, description) => true);
                    options.CustomSchemaIds(type => type.FullName);
                    // 添加一个安全方案定义
                    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header,
                        Description = "请输入JWT令牌"
                    });
                    // 将安全要求应用到所有的API上
                    options.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new List<string>()
                        }
                    });
                }
            );

            ConfigureCache(context, configuration);
            ConfigureDataProtection(context, configuration);
            ConfigureAuthentication(context, configuration);
            ConfigureCors(context, configuration);
        }

        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            PreConfigure<AbpAspNetCoreMvcOptions>(options =>
            {
                options.ConventionalControllers.Create(typeof(ApplicationModule).Assembly);
            });
        }

        public override void OnApplicationInitialization(
            ApplicationInitializationContext context)
        {
            var app = context.GetApplicationBuilder();
            var env = context.GetEnvironment();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseSwagger();

            app.UseAbpSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Stargazer API");
            });

            app.UseCors(DefaultCorsPolicyName);

            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseConfiguredEndpoints();

            if (env.IsProduction())
            {
                app.UseSpa(spa =>
                {
                    spa.Options.SourcePath = "wwwroot";
                });
            }
        }
    }
}

