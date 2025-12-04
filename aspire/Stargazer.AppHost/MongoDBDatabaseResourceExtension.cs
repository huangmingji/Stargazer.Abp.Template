using System.Runtime.InteropServices;

namespace Stargazer.AppHost;

public static class MongoDBDatabaseResourceExtension
{
    private static IResourceBuilder<MongoDBServerResource> Mongo { get; set; }

    public static IResourceBuilder<MongoDBDatabaseResource> CreateMongoDBDatabaseResource(
        this IDistributedApplicationBuilder builder)
    {
        string tag = "7-jammy";
        Architecture architecture = RuntimeInformation.ProcessArchitecture;
        if (architecture == Architecture.Arm
            || architecture == Architecture.Arm64)
        {
            tag =  "7-jammy-arm64";
        }
        var mongoUser = builder.AddParameter("mongo-user", "root");
        var mongoPwd = builder.AddParameter("mongo-pwd", "123456");
        Mongo = builder.AddMongoDB("mongo", 27017, mongoUser, mongoPwd)
            .WithContainerName("mongo")
            .WithImageRegistry("ccr.ccs.tencentyun.com")
            .WithImage("stargazer/mongo",tag)
            .WithBindMount("../../volumes/mongo/data", "/data/db")
            .WithMongoExpress(configureContainer =>
            {
                configureContainer.WithHostPort(8081);
            });
        return Mongo.AddDatabase("mongodb");
    }

    public static IResourceBuilder<MongoDBServerResource> GetMongoDBServerResource(this IResourceBuilder<MongoDBDatabaseResource> builder)
    {
        return Mongo;
    }
}