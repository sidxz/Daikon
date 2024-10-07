
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
COPY ["BuildingBlocks/Daikon.Shared/Daikon.Shared.csproj", "BuildingBlocks/Daikon.Shared/"]

# MLogix Services
COPY ["Services/MLogix/MLogix.API/MLogix.API.csproj", "Services/MLogix/MLogix.API/"]
COPY ["Services/MLogix/MLogix.Application/MLogix.Application.csproj", "Services/MLogix/MLogix.Application/"]
COPY ["Services/MLogix/MLogix.Domain/MLogix.Domain.csproj", "Services/MLogix/MLogix.Domain/"]
COPY ["Services/MLogix/MLogix.Infrastructure/MLogix.Infrastructure.csproj", "Services/MLogix/MLogix.Infrastructure/"]

RUN dotnet restore "Services/MLogix/MLogix.API/MLogix.API.csproj"
COPY . .

WORKDIR "/src/Services/MLogix/MLogix.API"
RUN dotnet build "MLogix.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MLogix.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MLogix.API.dll"]
