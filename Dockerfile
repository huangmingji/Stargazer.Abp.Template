
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

#copy csproj and restore as distinct layers
COPY Stargazer.Abp.Template.sln .
COPY nuget.config .
COPY src/Stargazer.Abp.Template.Application/*.csproj ./src/Stargazer.Abp.Template.Application/
COPY src/Stargazer.Abp.Template.Application.Contracts/*.csproj ./src/Stargazer.Abp.Template.Application.Contracts/
COPY src/Stargazer.Abp.Template.DbMigrations/*.csproj ./src/Stargazer.Abp.Template.DbMigrations/
COPY src/Stargazer.Abp.Template.Domain/*.csproj ./src/Stargazer.Abp.Template.Domain/
COPY src/Stargazer.Abp.Template.Domain.Shared/*.csproj ./src/Stargazer.Abp.Template.Domain.Shared/
COPY src/Stargazer.Abp.Template.EntityFrameworkCore/*.csproj ./src/Stargazer.Abp.Template.EntityFrameworkCore/
COPY src/Stargazer.Abp.Template.EntityFrameworkCore.DbMigrations/*.csproj ./src/Stargazer.Abp.Template.EntityFrameworkCore.DbMigrations/
COPY src/Stargazer.Abp.Template.HttpApi/*.csproj ./src/Stargazer.Abp.Template.HttpApi/
COPY src/Stargazer.Abp.Template.HttpApi.Client/*.csproj ./src/Stargazer.Abp.Template.HttpApi.Client/
COPY src/Stargazer.Abp.Template.Web/*.csproj ./src/Stargazer.Abp.Template.Web/
RUN dotnet restore /app/Stargazer.Abp.Template.sln -s https://nuget.cdn.azure.cn/v3/index.json

# # copy everything else and build app
WORKDIR /app/
COPY src/Stargazer.Abp.Template.Application/. ./src/Stargazer.Abp.Template.Application/
COPY src/Stargazer.Abp.Template.Application.Contracts/. ./src/Stargazer.Abp.Template.Application.Contracts/
COPY src/Stargazer.Abp.Template.DbMigrations/. ./src/Stargazer.Abp.Template.DbMigrations/
COPY src/Stargazer.Abp.Template.Domain/. ./src/Stargazer.Abp.Template.Domain/
COPY src/Stargazer.Abp.Template.Domain.Shared/. ./src/Stargazer.Abp.Template.Domain.Shared/
COPY src/Stargazer.Abp.Template.EntityFrameworkCore/. ./src/Stargazer.Abp.Template.EntityFrameworkCore/
COPY src/Stargazer.Abp.Template.EntityFrameworkCore.DbMigrations/. ./src/Stargazer.Abp.Template.EntityFrameworkCore.DbMigrations/
COPY src/Stargazer.Abp.Template.HttpApi/. ./src/Stargazer.Abp.Template.HttpApi/
COPY src/Stargazer.Abp.Template.HttpApi.Client/. ./src/Stargazer.Abp.Template.HttpApi.Client/
COPY src/Stargazer.Abp.Template.Web/. ./src/Stargazer.Abp.Template.Web/

WORKDIR /app/src/Stargazer.Abp.Template.Web
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/src/Stargazer.Abp.Template.Web/out ./

RUN ln -sf /usr/share/zoneinfo/Asia/Shanghai /etc/localtime
RUN echo 'Asia/Shanghai' >/etc/timezone

EXPOSE 8080
ENTRYPOINT ["dotnet", "Stargazer.Abp.Template.Web.dll", "--urls", "http://*:8080"]