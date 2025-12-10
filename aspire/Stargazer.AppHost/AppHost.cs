using Stargazer.AppHost;

var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.CreateRedisResource();
var postgresql = builder.CreatePostgresDatabaseResource();
var mongodb = builder.CreateMongoDBDatabaseResource();
var req = builder.CreateSeqResource();

var apiService = builder.AddProject<Projects.Stargazer_Abp_Template_Host>("apiservice")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health");

var webfrontend = builder.AddProject<Projects.Stargazer_Abp_Template_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(redis)
    .WaitFor(redis)
    .WithReference(mongodb)
    .WaitFor(mongodb)
    .WithReference(postgresql)
    .WaitFor(postgresql)
    .WithReference(req)
    .WaitFor(req)
    .WithReference(apiService)
    .WaitFor(apiService);

builder.CreateKeycloakResource()
        .WithReference(webfrontend)
        .WaitFor(webfrontend);

builder.Build().Run();
