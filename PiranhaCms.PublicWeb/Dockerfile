#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["PiranhaCms.PublicWeb/nuget.config", "PiranhaCms.PublicWeb/"]
COPY ["PiranhaCms.PublicWeb/PiranhaCMS.PublicWeb.csproj", "PiranhaCms.PublicWeb/"]
COPY ["PiranhaCMS.Extensions/PiranhaCMS.Common.csproj", "PiranhaCMS.Extensions/"]
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