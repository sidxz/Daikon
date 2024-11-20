
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

# DocuStore Services
COPY ["Services/DocuStore/DocuStore.API/DocuStore.API.csproj", "Services/DocuStore/DocuStore.API/"]
COPY ["Services/DocuStore/DocuStore.Application/DocuStore.Application.csproj", "Services/DocuStore/DocuStore.Application/"]
COPY ["Services/DocuStore/DocuStore.Domain/DocuStore.Domain.csproj", "Services/DocuStore/DocuStore.Domain/"]
COPY ["Services/DocuStore/DocuStore.Infrastructure/DocuStore.Infrastructure.csproj", "Services/DocuStore/DocuStore.Infrastructure/"]

RUN dotnet restore "Services/DocuStore/DocuStore.API/DocuStore.API.csproj"
COPY . .

WORKDIR "/src/Services/DocuStore/DocuStore.API"
RUN dotnet build "DocuStore.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DocuStore.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DocuStore.API.dll"]
