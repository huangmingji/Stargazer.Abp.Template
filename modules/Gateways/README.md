## 配置 HTTPS
https://learn.microsoft.com/zh-cn/aspnet/core/fundamentals/servers/kestrel/endpoints?view=aspnetcore-8.0#configure-https

## Linux部署
1. 复制打包的文件到目标服务器，解压
2. 创建服务配置文件：/etc/systemd/system/gateway.service:
```bash
[Unit]
Description=Gateway Service
After=network-online.target

[Service]
ExecStart=dotnet /<your path>/Stargazer.Abp.Template.Web.dll
User=root
Group=root
Restart=always
RestartSec=3
Environment="PATH=$PATH"

[Install]
WantedBy=default.target
```
然后启动服务
```bash
sudo systemctl daemon-reload
sudo systemctl enable gateway
sudo systemctl start gateway
```