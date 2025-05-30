
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

WORKDIR /app
EXPOSE 80
EXPOSE 443
# 💡 Fix: Disable tiered compilation at runtime (prevents RX→RW mapping issues)
ENV DOTNET_TieredCompilation=0
ENV DOTNET_ReadyToRun=0
ENV COMPlus_ReadyToRun=0

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Core Services
COPY ["BuildingBlocks/CQRS.Core/CQRS.Core.csproj", "BuildingBlocks/CQRS.Core/"]
COPY ["BuildingBlocks/Daikon.Events/Daikon.Events.csproj", "BuildingBlocks/Daikon.Events/"]
COPY ["BuildingBlocks/Daikon.EventStore/Daikon.EventStore.csproj", "BuildingBlocks/Daikon.EventStore/"]
COPY ["BuildingBlocks/Daikon.Shared/Daikon.Shared.csproj", "BuildingBlocks/Daikon.Shared/"]

# Extensions Services EventManagement
COPY ["Extensions/Daikon.EventManagement/Daikon.EventManagement.csproj", "Extensions/Daikon.EventManagement/Daikon.EventManagement.csproj"]

RUN dotnet restore "Extensions/Daikon.EventManagement/Daikon.EventManagement.csproj"
COPY . .

WORKDIR "/src/Extensions/Daikon.EventManagement"
RUN dotnet build "Daikon.EventManagement.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Daikon.EventManagement.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Daikon.EventManagement.dll"]
