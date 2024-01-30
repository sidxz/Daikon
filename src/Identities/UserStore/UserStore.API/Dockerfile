
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Core Services
COPY ["BuildingBlocks/CQRS.Core/CQRS.Core.csproj", "BuildingBlocks/CQRS.Core/"]

# UserStore Services
COPY ["Identities/UserStore/UserStore.API/UserStore.API.csproj", "Identities/UserStore/UserStore.API/"]
COPY ["Identities/UserStore/UserStore.Application/UserStore.Application.csproj", "Identities/UserStore/UserStore.Application/"]
COPY ["Identities/UserStore/UserStore.Domain/UserStore.Domain.csproj", "Identities/UserStore/UserStore.Domain/"]
COPY ["Identities/UserStore/UserStore.Infrastructure/UserStore.Infrastructure.csproj", "Identities/UserStore/UserStore.Infrastructure/"]

RUN dotnet restore "Identities/UserStore/UserStore.API/UserStore.API.csproj"
COPY . .

WORKDIR "/src/Identities/UserStore/UserStore.API"
RUN dotnet build "UserStore.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UserStore.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UserStore.API.dll"]
