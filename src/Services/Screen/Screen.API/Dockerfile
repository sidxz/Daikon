
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src


# # Install vsdbg for debugging
# RUN apt-get update && apt-get install -y unzip procps \
#     && rm -rf /var/lib/apt/lists/* \
#     && curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v latest -l /vsdbg

# Core Services
COPY ["BuildingBlocks/CQRS.Core/CQRS.Core.csproj", "BuildingBlocks/CQRS.Core/"]
COPY ["BuildingBlocks/Daikon.Events/Daikon.Events.csproj", "BuildingBlocks/Daikon.Events/"]
COPY ["BuildingBlocks/Daikon.EventStore/Daikon.EventStore.csproj", "BuildingBlocks/Daikon.EventStore/"]
COPY ["BuildingBlocks/Daikon.VersionStore/Daikon.VersionStore.csproj", "BuildingBlocks/Daikon.VersionStore/"]
COPY ["BuildingBlocks/Daikon.Shared/Daikon.Shared.csproj", "BuildingBlocks/Daikon.Shared/"]


# Screen Services
COPY ["Services/Screen/Screen.API/Screen.API.csproj", "Services/Screen/Screen.API/"]
COPY ["Services/Screen/Screen.Application/Screen.Application.csproj", "Services/Screen/Screen.Application/"]
COPY ["Services/Screen/Screen.Domain/Screen.Domain.csproj", "Services/Screen/Screen.Domain/"]
COPY ["Services/Screen/Screen.Infrastructure/Screen.Infrastructure.csproj", "Services/Screen/Screen.Infrastructure/"]

RUN dotnet restore "Services/Screen/Screen.API/Screen.API.csproj"
COPY . .

WORKDIR "/src/Services/Screen/Screen.API"
RUN dotnet build "Screen.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Screen.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Screen.API.dll"]
