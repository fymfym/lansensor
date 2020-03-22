FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["LanSensor.PollingMonitor/LanSensor.PollingMonitor.csproj", "LanSensor.PollingMonitor/"]
COPY ["LanSensor.PollingMonitor.Application/LanSensor.PollingMonitor.Application.csproj", "LanSensor.PollingMonitor.Application/"]
COPY ["LanSensor.PollingMonitor.Domain/LanSensor.PollingMonitor.Domain.csproj", "LanSensor.PollingMonitor.Domain/"]
COPY ["LanSensor.PollingMonitor.Infrastructure/LanSensor.PollingMonitor.Infrastructure.csproj", "LanSensor.PollingMonitor.Infrastructure.Repository/"]
COPY ["LanSensor.PollingMonitor.Test/LanSensor.PollingMonitor.Test.csproj", "LanSensor.PollingMonitor.Test.Repository/"]
COPY . .
RUN dotnet restore "LanSensor.PollingMonitor/LanSensor.PollingMonitor.csproj"
WORKDIR "/src/LanSensor.PollingMonitor"
RUN dotnet build "LanSensor.PollingMonitor.csproj" --source "/;https://api.nuget.org/v3/index.json" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LanSensor.PollingMonitor.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LanSensor.PollingMonitor.dll"]