using System.Runtime.InteropServices;

namespace Stargazer.AppHost;

public static class OllamaHostingExtension
{
    public static IResourceBuilder<OllamaResource> CreateOllamaResource(
        this IDistributedApplicationBuilder builder)
    {
        string tag = "0.13.0";
        Architecture architecture = RuntimeInformation.ProcessArchitecture;
        if (architecture == Architecture.Arm
            || architecture == Architecture.Arm64)
        {
            tag =  "0.13.0-arm64";
        }
        return builder.AddOllama("ollama")
            .WithContainerName("ollama")
            .WithImageRegistry("ccr.ccs.tencentyun.com")
            .WithImage("stargazer/ollama",tag);
    }
    
    public static IResourceBuilder<OllamaModelResource> CreateOllamaModelResource(
        this IResourceBuilder<OllamaResource> builder, string modelName)
    {
        return builder.AddModel(modelName);
    }
}