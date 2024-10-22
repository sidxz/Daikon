
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Core Services
COPY ["BuildingBlocks/CQRS.Core/CQRS.Core.csproj", "BuildingBlocks/CQRS.Core/"]
COPY ["BuildingBlocks/Daikon.Events/Daikon.Events.csproj", "BuildingBlocks/Daikon.Events/"]
COPY ["BuildingBlocks/Daikon.EventStore/Daikon.EventStore.csproj", "BuildingBlocks/Daikon.EventStore/"]
COPY ["BuildingBlocks/Daikon.VersionStore/Daikon.VersionStore.csproj", "BuildingBlocks/Daikon.VersionStore/"]

# EventHistory Services
COPY ["Services/EventHistory/EventHistory.API/EventHistory.API.csproj", "Services/EventHistory/EventHistory.API/"]
COPY ["Services/EventHistory/EventHistory.Application/EventHistory.Application.csproj", "Services/EventHistory/EventHistory.Application/"]
COPY ["Services/EventHistory/EventHistory.Domain/EventHistory.Domain.csproj", "Services/EventHistory/EventHistory.Domain/"]
COPY ["Services/EventHistory/EventHistory.Infrastructure/EventHistory.Infrastructure.csproj", "Services/EventHistory/EventHistory.Infrastructure/"]

RUN dotnet restore "Services/EventHistory/EventHistory.API/EventHistory.API.csproj"
COPY . .

WORKDIR "/src/Services/EventHistory/EventHistory.API"
RUN dotnet build "EventHistory.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EventHistory.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EventHistory.API.dll"]
