
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

# Comment Services
COPY ["Services/Comment/Comment.API/Comment.API.csproj", "Services/Comment/Comment.API/"]
COPY ["Services/Comment/Comment.Application/Comment.Application.csproj", "Services/Comment/Comment.Application/"]
COPY ["Services/Comment/Comment.Domain/Comment.Domain.csproj", "Services/Comment/Comment.Domain/"]
COPY ["Services/Comment/Comment.Infrastructure/Comment.Infrastructure.csproj", "Services/Comment/Comment.Infrastructure/"]

RUN dotnet restore "Services/Comment/Comment.API/Comment.API.csproj"
COPY . .

WORKDIR "/src/Services/Comment/Comment.API"
RUN dotnet build "Comment.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Comment.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Comment.API.dll"]
