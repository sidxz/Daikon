
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
COPY ["BuildingBlocks/Daikon.Shared/Daikon.Shared.csproj", "BuildingBlocks/Daikon.Shared/"]


# Questionnaire Services
COPY ["Services/Questionnaire/Questionnaire.API/Questionnaire.API.csproj", "Services/Questionnaire/Questionnaire.API/"]
COPY ["Services/Questionnaire/Questionnaire.Application/Questionnaire.Application.csproj", "Services/Questionnaire/Questionnaire.Application/"]
COPY ["Services/Questionnaire/Questionnaire.Domain/Questionnaire.Domain.csproj", "Services/Questionnaire/Questionnaire.Domain/"]
COPY ["Services/Questionnaire/Questionnaire.Infrastructure/Questionnaire.Infrastructure.csproj", "Services/Questionnaire/Questionnaire.Infrastructure/"]

RUN dotnet restore "Services/Questionnaire/Questionnaire.API/Questionnaire.API.csproj"
COPY . .

WORKDIR "/src/Services/Questionnaire/Questionnaire.API"
RUN dotnet build "Questionnaire.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Questionnaire.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Questionnaire.API.dll"]
