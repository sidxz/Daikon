
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
COPY ["BuildingBlocks/Daikon.Shared/Daikon.Shared.csproj", "BuildingBlocks/Daikon.Shared/"]

# Aggregators Services
COPY ["Aggregators/Aggregators.API/Aggregators.API.csproj", "Aggregators/Aggregators.API/"]
COPY ["Aggregators/Aggregators.Application/Aggregators.Application.csproj", "Aggregators/Aggregators.Application/"]

RUN dotnet restore "Aggregators/Aggregators.API/Aggregators.API.csproj"
COPY . .

WORKDIR "/src/Aggregators/Aggregators.API"
RUN dotnet build "Aggregators.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Aggregators.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Aggregators.API.dll"]
