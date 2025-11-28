using System.Runtime.InteropServices;
using Stargazer.AppHost;

var builder = DistributedApplication.CreateBuilder(args);


var redis = builder.CreateRedisResource();
var postgresql = builder.CreatePostgresDatabaseResource();
var mongodb = builder.CreateMongoDBDatabaseResource();
var rabbitmq = builder.CreateRabbitMQResource();

IResourceBuilder<ProjectResource> apiService = builder.AddProject<Projects.Stargazer_Abp_Template_Host>("api-service");

builder.AddProject<Projects.Stargazer_Abp_Template_Web>("frontend")
    .WithExternalHttpEndpoints()
    .WithReference(redis)
    .WithReference(postgresql)
    .WithReference(mongodb)
    .WaitFor(redis)
    .WaitFor(postgresql.GetPostgresServerResource())
    .WaitFor(mongodb)
    .WithReference(apiService);


// var frontend = builder.AddNpmApp("frontend", "../../../react/frontend", "dev")
//     .WithHttpEndpoint(port:3001, env: "PORT")
//     .WithEnvironment(context =>
//     {
//         context.EnvironmentVariables["NODE_ENV"] = "development";
//     })
//     .WithReference(apiService)
//     .WaitFor(apiService);

builder.Build().Run();
