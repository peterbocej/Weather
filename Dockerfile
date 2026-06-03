# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5000
EXPOSE 5001


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Weather.Api/Weather.Api.csproj", "Weather.Api/"]
COPY ["Weather.Application/Weather.Application.csproj", "Weather.Application/"]
COPY ["Weather.Infrastructure/Weather.Infrastructure.csproj", "Weather.Infrastructure/"]
COPY ["Weather.Domain/Weather.Domain.csproj", "Weather.Domain/"]
RUN dotnet restore "./Weather.Api/Weather.Api.csproj"
COPY . .
WORKDIR "/src/Weather.Api"
RUN dotnet build "./Weather.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Weather.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
RUN mkdir /app/Data
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Weather.Api.dll"]