
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

# Target Services
COPY ["Services/Target/Target.API/Target.API.csproj", "Services/Target/Target.API/"]
COPY ["Services/Target/Target.Application/Target.Application.csproj", "Services/Target/Target.Application/"]
COPY ["Services/Target/Target.Domain/Target.Domain.csproj", "Services/Target/Target.Domain/"]
COPY ["Services/Target/Target.Infrastructure/Target.Infrastructure.csproj", "Services/Target/Target.Infrastructure/"]

RUN dotnet restore "Services/Target/Target.API/Target.API.csproj"
COPY . .

WORKDIR "/src/Services/Target/Target.API"
RUN dotnet build "Target.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Target.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Target.API.dll"]
