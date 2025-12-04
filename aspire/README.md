# 安装 .NET Aspire

参考官方文档：
[https://learn.microsoft.com/zh-cn/dotnet/aspire/fundamentals/setup-tooling?tabs=linux&pivots=dotnet-cli#install-net-aspire](https://learn.microsoft.com/zh-cn/dotnet/aspire/fundamentals/setup-tooling?tabs=linux&pivots=dotnet-cli#install-net-aspire)

```bash
dotnet workload update
dotnet workload install aspire
dotnet workload list
```

# 构建应用

[https://learn.microsoft.com/zh-cn/dotnet/aspire/get-started/build-your-first-aspire-app?pivots=dotnet-cli](https://learn.microsoft.com/zh-cn/dotnet/aspire/get-started/build-your-first-aspire-app?pivots=dotnet-cli)

```bash
dotnet new aspire-starter --use-redis-cache --output AspireApplication
```
