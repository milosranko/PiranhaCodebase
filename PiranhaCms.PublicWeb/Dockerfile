#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["global.json", "Directory.Build.props", "./"]
COPY ["PiranhaCms.PublicWeb/nuget.config", "PiranhaCms.PublicWeb/"]
COPY ["PiranhaCms.PublicWeb/PiranhaCMS.PublicWeb.csproj", "PiranhaCms.PublicWeb/"]
COPY ["PiranhaCMS.Business/PiranhaCMS.Business.csproj", "PiranhaCMS.Business/"]
COPY ["PiranhaCMS.ContentTypes/PiranhaCMS.ContentTypes.csproj", "PiranhaCMS.ContentTypes/"]
COPY ["PiranhaCMS.Extensions/PiranhaCMS.Common.csproj", "PiranhaCMS.Extensions/"]
COPY ["PiranhaCMS.ImageCache/PiranhaCMS.ImageCache.csproj", "PiranhaCMS.ImageCache/"]
COPY ["PiranhaCMS.Search/PiranhaCMS.Search.csproj", "PiranhaCMS.Search/"]
COPY ["PiranhaCMS.Validators/PiranhaCMS.Validators.csproj", "PiranhaCMS.Validators/"]
RUN dotnet restore "PiranhaCms.PublicWeb/PiranhaCMS.PublicWeb.csproj"
COPY . .
WORKDIR "/src/PiranhaCms.PublicWeb"
RUN dotnet build "PiranhaCMS.PublicWeb.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PiranhaCMS.PublicWeb.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PiranhaCMS.PublicWeb.dll"]
