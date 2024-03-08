
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Gateway Services
COPY ["ApiGateways/SimpleGW/SimpleGW.API/SimpleGW.API.csproj", "ApiGateways/SimpleGW/SimpleGW.API/"]

RUN dotnet restore "ApiGateways/SimpleGW/SimpleGW.API/SimpleGW.API.csproj"

COPY . .

WORKDIR "/src/ApiGateways/SimpleGW/SimpleGW.API/"
RUN dotnet build "SimpleGW.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SimpleGW.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SimpleGW.API.dll"]
