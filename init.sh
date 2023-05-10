#!/bin/sh

ProjectModule=$1
if [ -z "$ProjectModule" ]
then
    echo "请输入参数"
    exit
fi

#创建解决方案
dotnet new sln -n $ProjectModule

mkdir -p ./src

#创建模块分层
dotnet new classlib -o ./src/$ProjectModule.Application
dotnet new classlib -o ./src/$ProjectModule.Application.Contracts
dotnet new console -o ./src/$ProjectModule.DbMigrations
dotnet new classlib -o ./src/$ProjectModule.Domain
dotnet new classlib -o ./src/$ProjectModule.Domain.Shared
dotnet new classlib -o ./src/$ProjectModule.EntityFrameworkCore
dotnet new classlib -o ./src/$ProjectModule.EntityFrameworkCore.DbMigrations
dotnet new classlib -o ./src/$ProjectModule.HttpApi
dotnet new classlib -o ./src/$ProjectModule.HttpApi.Client
dotnet new webapi -o ./src/$ProjectModule.Host

#加入解决方案
dotnet sln $ProjectModule.sln add ./src/$ProjectModule.Domain.Shared
dotnet sln $ProjectModule.sln add ./src/$ProjectModule.Domain
dotnet sln $ProjectModule.sln add ./src/$ProjectModule.DbMigrations
dotnet sln $ProjectModule.sln add ./src/$ProjectModule.EntityFrameworkCore
dotnet sln $ProjectModule.sln add ./src/$ProjectModule.EntityFrameworkCore.DbMigrations
dotnet sln $ProjectModule.sln add ./src/$ProjectModule.Application.Contracts
dotnet sln $ProjectModule.sln add ./src/$ProjectModule.Application
dotnet sln $ProjectModule.sln add ./src/$ProjectModule.HttpApi
dotnet sln $ProjectModule.sln add ./src/$ProjectModule.HttpApi.Client
dotnet sln $ProjectModule.sln add ./src/$ProjectModule.Host

#分层间互相引用
dotnet add ./src/$ProjectModule.Domain/$ProjectModule.Domain.csproj reference ./src/$ProjectModule.Domain.Shared/$ProjectModule.Domain.Shared.csproj

dotnet add ./src/$ProjectModule.EntityFrameworkCore/$ProjectModule.EntityFrameworkCore.csproj reference ./src/$ProjectModule.Domain/$ProjectModule.Domain.csproj

dotnet add ./src/$ProjectModule.EntityFrameworkCore.DbMigrations/$ProjectModule.EntityFrameworkCore.DbMigrations.csproj reference ./src/$ProjectModule.EntityFrameworkCore/$ProjectModule.EntityFrameworkCore.csproj

dotnet add ./src/$ProjectModule.Application/$ProjectModule.Application.csproj reference ./src/$ProjectModule.Application.Contracts/$ProjectModule.Application.Contracts.csproj
dotnet add ./src/$ProjectModule.Application/$ProjectModule.Application.csproj reference ./src/$ProjectModule.Domain/$ProjectModule.Domain.csproj

dotnet add ./src/$ProjectModule.HttpApi/$ProjectModule.HttpApi.csproj reference ./src/$ProjectModule.Application.Contracts/$ProjectModule.Application.Contracts.csproj

dotnet add ./src/$ProjectModule.HttpApi.Client/$ProjectModule.HttpApi.Client.csproj reference ./src/$ProjectModule.Application.Contracts/$ProjectModule.Application.Contracts.csproj

dotnet add ./src/$ProjectModule.Host/$ProjectModule.Host.csproj reference ./src/$ProjectModule.EntityFrameworkCore/$ProjectModule.EntityFrameworkCore.csproj
dotnet add ./src/$ProjectModule.Host/$ProjectModule.Host.csproj reference ./src/$ProjectModule.Application/$ProjectModule.Application.csproj
dotnet add ./src/$ProjectModule.Host/$ProjectModule.Host.csproj reference ./src/$ProjectModule.HttpApi/$ProjectModule.HttpApi.csproj

dotnet add ./src/$ProjectModule.DbMigrations/$ProjectModule.DbMigrations.csproj reference ./src/$ProjectModule.Application/$ProjectModule.Application.csproj
dotnet add ./src/$ProjectModule.DbMigrations/$ProjectModule.DbMigrations.csproj reference ./src/$ProjectModule.EntityFrameworkCore.DbMigrations/$ProjectModule.EntityFrameworkCore.DbMigrations.csproj

#添加nuget依赖包
dotnet add ./src/$ProjectModule.Domain.Shared/$ProjectModule.Domain.Shared.csproj package Volo.Abp.Validation
dotnet add ./src/$ProjectModule.Domain/$ProjectModule.Domain.csproj package Volo.Abp.Ddd.Domain
dotnet add ./src/$ProjectModule.Domain.Shared/$ProjectModule.Domain.Shared.csproj package Volo.Abp.Localization
dotnet add ./src/$ProjectModule.Domain/$ProjectModule.Domain.csproj package Lemon.Common

dotnet add ./src/$ProjectModule.EntityFrameworkCore/$ProjectModule.EntityFrameworkCore.csproj package Volo.Abp.EntityFrameworkCore
dotnet add ./src/$ProjectModule.EntityFrameworkCore/$ProjectModule.EntityFrameworkCore.csproj package Volo.Abp.EntityFrameworkCore.PostgreSql

dotnet add ./src/$ProjectModule.EntityFrameworkCore.DbMigrations/$ProjectModule.EntityFrameworkCore.DbMigrations.csproj package Volo.Abp.EntityFrameworkCore
dotnet add ./src/$ProjectModule.EntityFrameworkCore.DbMigrations/$ProjectModule.EntityFrameworkCore.DbMigrations.csproj package Volo.Abp.EntityFrameworkCore.PostgreSql
dotnet add ./src/$ProjectModule.EntityFrameworkCore.DbMigrations/$ProjectModule.EntityFrameworkCore.DbMigrations.csproj package Microsoft.EntityFrameworkCore.Design

dotnet add ./src/$ProjectModule.Application.Contracts/$ProjectModule.Application.Contracts.csproj package Volo.Abp.Ddd.Application.Contracts
dotnet add ./src/$ProjectModule.Application.Contracts/$ProjectModule.Application.Contracts.csproj package Volo.Abp.FluentValidation

dotnet add ./src/$ProjectModule.Application/$ProjectModule.Application.csproj package Volo.Abp.Ddd.Application
dotnet add ./src/$ProjectModule.Application/$ProjectModule.Application.csproj package Volo.Abp.AutoMapper
dotnet add ./src/$ProjectModule.Application/$ProjectModule.Application.csproj package Volo.Abp.Caching

dotnet add ./src/$ProjectModule.HttpApi/$ProjectModule.HttpApi.csproj package Volo.Abp.AspNetCore.Mvc

dotnet add ./src/$ProjectModule.HttpApi.Client/$ProjectModule.HttpApi.Client.csproj package Volo.Abp.Http.Client

dotnet add ./src/$ProjectModule.Host/$ProjectModule.Host.csproj package Volo.Abp.AspNetCore.Mvc
dotnet add ./src/$ProjectModule.Host/$ProjectModule.Host.csproj package Swashbuckle.AspNetCore
dotnet add ./src/$ProjectModule.Host/$ProjectModule.Host.csproj package Microsoft.AspNetCore.DataProtection.StackExchangeRedis
dotnet add ./src/$ProjectModule.Host/$ProjectModule.Host.csproj package Microsoft.Extensions.Caching.StackExchangeRedis
dotnet add ./src/$ProjectModule.Host/$ProjectModule.Host.csproj package Serilog.AspNetCore
dotnet add ./src/$ProjectModule.Host/$ProjectModule.Host.csproj package Serilog.Settings.Configuration
dotnet add ./src/$ProjectModule.Host/$ProjectModule.Host.csproj package Serilog.Sinks.Async
dotnet add ./src/$ProjectModule.Host/$ProjectModule.Host.csproj package Serilog.Sinks.File
dotnet add ./src/$ProjectModule.Host/$ProjectModule.Host.csproj package Volo.Abp.AspNetCore.Serilog
dotnet add ./src/$ProjectModule.Host/$ProjectModule.Host.csproj package Volo.Abp.Autofac

dotnet add ./src/$ProjectModule.DbMigrations/$ProjectModule.DbMigrations.csproj package Microsoft.Extensions.Hosting
dotnet add ./src/$ProjectModule.DbMigrations/$ProjectModule.DbMigrations.csproj package Volo.Abp.Autofac
dotnet add ./src/$ProjectModule.DbMigrations/$ProjectModule.DbMigrations.csproj package Serilog.Extensions.Logging
dotnet add ./src/$ProjectModule.DbMigrations/$ProjectModule.DbMigrations.csproj package Serilog.Sinks.File
dotnet add ./src/$ProjectModule.DbMigrations/$ProjectModule.DbMigrations.csproj package Serilog.Sinks.Console
dotnet add ./src/$ProjectModule.DbMigrations/$ProjectModule.DbMigrations.csproj package Microsoft.EntityFrameworkCore.Tools