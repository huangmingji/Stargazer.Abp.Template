using Stargazer.AppHost;

var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.CreateRedisResource();
var postgresql = builder.CreatePostgresDatabaseResource();
var mongodb = builder.CreateMongoDBDatabaseResource();
var req = builder.CreateSeqResource();
var keycloak = builder.CreateKeycloakResource();

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
    .WithReference(keycloak)
    .WaitFor(keycloak)
    .WithReference(req)
    .WaitFor(req)
    .WithReference(apiService)
    .WaitFor(apiService);

builder.AddProject<Projects.StargazerGateway>("gateway")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(webfrontend)
    .WaitFor(webfrontend);

builder.Build().Run();
