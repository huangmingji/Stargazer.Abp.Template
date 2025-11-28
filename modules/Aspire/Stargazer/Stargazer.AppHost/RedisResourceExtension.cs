using System.Runtime.InteropServices;

namespace Stargazer.AppHost;

public static class RedisResourceExtension
{
    public static IResourceBuilder<RedisResource> CreateRedisResource(this IDistributedApplicationBuilder builder)
    {
        string tag = "7-alpine";
        Architecture architecture = RuntimeInformation.ProcessArchitecture;
        if (architecture == Architecture.Arm
            || architecture == Architecture.Arm64)
        {
            tag =  "7-alpine-arm64";
        }
        return builder.AddRedis("redis", 6379)
            .WithContainerName("redis")
            .WithImageRegistry("ccr.ccs.tencentyun.com")
            .WithImage("stargazer/redis", tag)
            // .WithDataVolume("redis")
            .WithBindMount("../../../volumes/redis/data", "/data")
            .WithRedisCommander(configureContainer =>
            {
                configureContainer.WithImageRegistry("ccr.ccs.tencentyun.com")
                    .WithImage("stargazer/redis-commander", "latest")
                    .WithBindMount("../../../volumes/redis-commander/data", "/app/data");
            }, "redis-commander");
    }
}