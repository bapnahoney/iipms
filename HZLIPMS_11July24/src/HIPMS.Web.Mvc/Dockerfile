FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

RUN dotnet tool install -g Microsoft.Web.LibraryManager.Cli
RUN ln -s ~/.dotnet/tools/libman /usr/local/bin/libman

WORKDIR /src
COPY ["src/HIPMS.Web.Mvc/HIPMS.Web.Mvc.csproj", "src/HIPMS.Web.Mvc/"]
COPY ["src/HIPMS.Web.Core/HIPMS.Web.Core.csproj", "src/HIPMS.Web.Core/"]
COPY ["src/HIPMS.Application/HIPMS.Application.csproj", "src/HIPMS.Application/"]
COPY ["src/HIPMS.Core/HIPMS.Core.csproj", "src/HIPMS.Core/"]
COPY ["src/HIPMS.EntityFrameworkCore/HIPMS.EntityFrameworkCore.csproj", "src/HIPMS.EntityFrameworkCore/"]
WORKDIR "/src/src/HIPMS.Web.Mvc"
RUN dotnet restore 

WORKDIR /src
COPY ["src/HIPMS.Web.Mvc", "src/HIPMS.Web.Mvc"]
COPY ["src/HIPMS.Web.Core", "src/HIPMS.Web.Core"]
COPY ["src/HIPMS.Application", "src/HIPMS.Application"]
COPY ["src/HIPMS.Core", "src/HIPMS.Core"]
COPY ["src/HIPMS.EntityFrameworkCore", "src/HIPMS.EntityFrameworkCore"]
WORKDIR "/src/src/HIPMS.Web.Mvc"
RUN libman restore
RUN dotnet publish -c Release -o /publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:7.0
EXPOSE 80
WORKDIR /app
COPY --from=build /publish .
ENTRYPOINT ["dotnet", "HIPMS.Web.Mvc.dll"]
