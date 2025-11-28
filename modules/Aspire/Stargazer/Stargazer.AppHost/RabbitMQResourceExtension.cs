using System.Runtime.InteropServices;

namespace Stargazer.AppHost;

public static class RabbitMQResourceExtension
{
    public static IResourceBuilder<RabbitMQServerResource> CreateRabbitMQResource(this IDistributedApplicationBuilder builder)
    {
        string tag = "4-management";
        Architecture architecture = RuntimeInformation.ProcessArchitecture;
        if (architecture == Architecture.Arm
            || architecture == Architecture.Arm64)
        {
            tag =  "4-management-arm64";
        }
        var rabbitmqUser = builder.AddParameter("rabbitmq-user", "root", secret: true);
        var rabbitmqPwd = builder.AddParameter("rabbitmq-pwd", "123456", secret: true);
        return builder.AddRabbitMQ("rabbitmq", rabbitmqUser, rabbitmqPwd, 5672)
            .WithContainerName("rabbitmq")
            .WithImageRegistry("ccr.ccs.tencentyun.com")
            .WithImage("stargazer/rabbitmq", tag)
            .WithBindMount("../../../volumes/rabbitmq/data", "/var/lib/rabbitmq")
            .WithBindMount("../../../volumes/rabbitmq/etc", "/etc/rabbitmq")
            .WithBindMount("../../../volumes/rabbitmq/logs", "/var/log/rabbitmq");
    }
}