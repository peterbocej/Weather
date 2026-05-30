FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5555
EXPOSE 5556

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Weather.Api/Weather.Api.csproj", "Weather.Api/"]
RUN dotnet restore "Weather.Api/Weather.Api.csproj"
COPY . .
WORKDIR "/src/Weather.Api"
RUN dotnet build "Weather.Api.csproj" -c Release -o /app/build

FROM build AS publish
WORKDIR "/src/Weather.Api"
RUN dotnet publish "./Weather.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN mkdir -p /app/Data
ENTRYPOINT ["dotnet", "Weather.Api.dll"]