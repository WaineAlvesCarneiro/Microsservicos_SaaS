FROM mcr.microsoft.com/dotnet/aspnet:11.0 AS base
USER root

RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

USER $APP_UID
WORKDIR /app
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:11.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["IdentityService.API/IdentityService.API.csproj","IdentityService.API/"]
RUN dotnet restore "./IdentityService.API/IdentityService.API.csproj"
COPY . .
WORKDIR "/src/IdentityService.API"
RUN dotnet build "./IdentityService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./IdentityService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IdentityService.API.dll"]