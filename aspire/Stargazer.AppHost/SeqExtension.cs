using System.Runtime.InteropServices;

namespace Stargazer.AppHost;

public static class SeqExtension
{
    public static IResourceBuilder<SeqResource> CreateSeqResource(
        this IDistributedApplicationBuilder builder)
    {
        string tag = "2025.2";
        Architecture architecture = RuntimeInformation.ProcessArchitecture;
        if (architecture == Architecture.Arm
            || architecture == Architecture.Arm64)
        {
            tag =  "2025.2-arm64";
        }
        var password = builder.AddParameter("password", "123456");
        return builder.AddSeq("seq", password,5341)
            .WithContainerName("seq")
            .WithImageRegistry("ccr.ccs.tencentyun.com")
            .WithImage("stargazer/seq",tag)
            .ExcludeFromManifest()
            .WithLifetime(ContainerLifetime.Persistent)
            .WithEnvironment("ACCEPT_EULA", "Y")
            .WithDataBindMount("../../volumes/seq/data");
    }
}