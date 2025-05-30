
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

# UserPreferences Services
COPY ["Services/UserPreferences/UserPreferences.API/UserPreferences.API.csproj", "Services/UserPreferences/UserPreferences.API/"]
COPY ["Services/UserPreferences/UserPreferences.Application/UserPreferences.Application.csproj", "Services/UserPreferences/UserPreferences.Application/"]
COPY ["Services/UserPreferences/UserPreferences.Domain/UserPreferences.Domain.csproj", "Services/UserPreferences/UserPreferences.Domain/"]
COPY ["Services/UserPreferences/UserPreferences.Infrastructure/UserPreferences.Infrastructure.csproj", "Services/UserPreferences/UserPreferences.Infrastructure/"]

RUN dotnet restore "Services/UserPreferences/UserPreferences.API/UserPreferences.API.csproj"
COPY . .

WORKDIR "/src/Services/UserPreferences/UserPreferences.API"
RUN dotnet build "UserPreferences.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UserPreferences.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UserPreferences.API.dll"]
