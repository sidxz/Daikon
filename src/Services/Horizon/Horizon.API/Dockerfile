
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

# Horizon Services
COPY ["Services/Horizon/Horizon.API/Horizon.API.csproj", "Services/Horizon/Horizon.API/"]
COPY ["Services/Horizon/Horizon.Application/Horizon.Application.csproj", "Services/Horizon/Horizon.Application/"]
COPY ["Services/Horizon/Horizon.Domain/Horizon.Domain.csproj", "Services/Horizon/Horizon.Domain/"]
COPY ["Services/Horizon/Horizon.Infrastructure/Horizon.Infrastructure.csproj", "Services/Horizon/Horizon.Infrastructure/"]

RUN dotnet restore "Services/Horizon/Horizon.API/Horizon.API.csproj"
COPY . .

WORKDIR "/src/Services/Horizon/Horizon.API"
RUN dotnet build "Horizon.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Horizon.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Horizon.API.dll"]
