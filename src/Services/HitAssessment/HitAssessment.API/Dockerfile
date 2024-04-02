
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

# HitAssessment Services
COPY ["Services/HitAssessment/HitAssessment.API/HitAssessment.API.csproj", "Services/HitAssessment/HitAssessment.API/"]
COPY ["Services/HitAssessment/HitAssessment.Application/HitAssessment.Application.csproj", "Services/HitAssessment/HitAssessment.Application/"]
COPY ["Services/HitAssessment/HitAssessment.Domain/HitAssessment.Domain.csproj", "Services/HitAssessment/HitAssessment.Domain/"]
COPY ["Services/HitAssessment/HitAssessment.Infrastructure/HitAssessment.Infrastructure.csproj", "Services/HitAssessment/HitAssessment.Infrastructure/"]

RUN dotnet restore "Services/HitAssessment/HitAssessment.API/HitAssessment.API.csproj"
COPY . .

WORKDIR "/src/Services/HitAssessment/HitAssessment.API"
RUN dotnet build "HitAssessment.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HitAssessment.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HitAssessment.API.dll"]
