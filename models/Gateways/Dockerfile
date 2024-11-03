
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

#copy csproj and restore as distinct layers
COPY Gateways.sln .
COPY StargazerGateway/*.csproj ./StargazerGateway/
RUN dotnet restore /app/Gateways.sln -s https://nuget.cdn.azure.cn/v3/index.json

# # copy everything else and build app
COPY StargazerGateway/. ./StargazerGateway/
WORKDIR /app/StargazerGateway
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/StargazerGateway/out ./

RUN ln -sf /usr/share/zoneinfo/Asia/Shanghai /etc/localtime
RUN echo 'Asia/Shanghai' >/etc/timezone

EXPOSE 80 443
ENTRYPOINT ["dotnet", "Stargazer.Abp.Template.Web.dll", "--urls", "http://*:80;https://*:443"]