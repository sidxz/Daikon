
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

# Project Services
COPY ["Services/Project/Project.API/Project.API.csproj", "Services/Project/Project.API/"]
COPY ["Services/Project/Project.Application/Project.Application.csproj", "Services/Project/Project.Application/"]
COPY ["Services/Project/Project.Domain/Project.Domain.csproj", "Services/Project/Project.Domain/"]
COPY ["Services/Project/Project.Infrastructure/Project.Infrastructure.csproj", "Services/Project/Project.Infrastructure/"]

RUN dotnet restore "Services/Project/Project.API/Project.API.csproj"
COPY . .

WORKDIR "/src/Services/Project/Project.API"
RUN dotnet build "Project.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Project.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Project.API.dll"]
