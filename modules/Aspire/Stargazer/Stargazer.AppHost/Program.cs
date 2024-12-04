using System.Runtime.InteropServices;

var builder = DistributedApplication.CreateBuilder(args);

string redisImage = "hub.atomgit.com/amd64/redis";
string postgresqlImage = "hub.atomgit.com/amd64/postgres";
string mongodbImage = "hub.atomgit.com/amd64/mongo";
Architecture architecture = RuntimeInformation.ProcessArchitecture;
if(architecture == Architecture.Arm
   || architecture == Architecture.Arm64)
{
    redisImage = "hub.atomgit.com/arm64v8/redis";
    postgresqlImage = "hub.atomgit.com/arm64v8/postgres";
    mongodbImage = "hub.atomgit.com/arm64v8/mongo";
}
    
var redis = builder.AddRedis("redis", 6379)
    .WithContainerName("redis")
    .WithImage(redisImage, "7-alpine")
    .WithDataVolume("redis")
    .WithRedisCommander(null, "redis-commander");

var username = builder.AddParameter("postgres-uid", "postgres");
var password = builder.AddParameter("postgres-pwd", "123456");
var postgres = builder.AddPostgres("postgres", username, password, 5432)
    .WithContainerName("postgres")
    .WithImage(postgresqlImage, "15-alpine")
    .WithDataVolume("postgres");
var postgresql = postgres.AddDatabase("postgresql");

var mongoUser = builder.AddParameter("mongo-user", "root");
var mongoPwd = builder.AddParameter("mongo-pwd", "123456");
var mongo = builder.AddMongoDB("mongo", 27017, mongoUser, mongoPwd)
    .WithContainerName("mongo")
    .WithImage(mongodbImage, "7-jammy")
    .WithDataVolume("mongo");
var mongodb = mongo.AddDatabase("mongodb");

IResourceBuilder<ProjectResource> apiService = builder.AddProject<Projects.Stargazer_Abp_Template_Host>("api-service");
IResourceBuilder<ProjectResource> webService = builder.AddProject<Projects.Stargazer_Abp_Template_Web>("web-service");

webService.WithExternalHttpEndpoints()
    .WithReference(redis)
    .WithReference(postgresql)
    .WithReference(mongodb)
    .WaitFor(redis)
    .WaitFor(postgres)
    .WaitFor(mongodb)
    .WithReference(apiService);

builder.Build().Run();
