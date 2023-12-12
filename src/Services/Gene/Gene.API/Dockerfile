
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

# Gene Services
COPY ["Services/Gene/Gene.API/Gene.API.csproj", "Services/Gene/Gene.API/"]
COPY ["Services/Gene/Gene.Application/Gene.Application.csproj", "Services/Gene/Gene.Application/"]
COPY ["Services/Gene/Gene.Domain/Gene.Domain.csproj", "Services/Gene/Gene.Domain/"]
COPY ["Services/Gene/Gene.Infrastructure/Gene.Infrastructure.csproj", "Services/Gene/Gene.Infrastructure/"]

RUN dotnet restore "Services/Gene/Gene.API/Gene.API.csproj"
COPY . .

WORKDIR "/src/Services/Gene/Gene.API"
RUN dotnet build "Gene.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Gene.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Gene.API.dll"]
