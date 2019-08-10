# 使用sdk 2.2作为创建环境
FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
# 使用app文件夹作为目录
WORKDIR /app

# 拷贝解决方案到app下
COPY *.sln .
# 拷贝项目文件
COPY dnc.spider.webapi/*.csproj ./dnc.spider.webapi/
COPY dnc.efcontext/dnc.efcontext.csproj ./dnc.efcontext/
COPY dnc.model/dnc.model.csproj ./dnc.model/
COPY dnc.spider/dnc.spider.csproj ./dnc.spider/
COPY dnc.spider.helper/dnc.spider.helper.csproj ./dnc.spider.helper/
# 执行还原
RUN dotnet restore 

# 把所有文件拷贝进去
COPY dnc.spider.webapi/. ./dnc.spider.webapi/
COPY dnc.efcontext/. ./dnc.efcontext/
COPY dnc.model/. ./dnc.model/
COPY dnc.spider/. ./dnc.spider/
COPY dnc.spider.helper/. ./dnc.spider.helper/
# 切换到/app/dnc.spider.webapi目录
WORKDIR /app/dnc.spider.webapi
# 执行发布命令，把发布的文件放在out文件夹
RUN dotnet publish -c Release -o out


# 使用2.2运行时作为创建环境
FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS runtime
# 使用app文件夹作为目录
WORKDIR /app
# 从build环境中拷贝发布文件夹到app目录
COPY --from=build /app/dnc.spider.webapi/out ./
# 执行命令
ENTRYPOINT [ "dotnet",  "dnc.spider.webapi.dll" ]




