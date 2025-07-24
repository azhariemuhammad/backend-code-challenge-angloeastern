FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["ShipManagement/ShipManagement.csproj", "ShipManagement/"]
RUN dotnet restore "ShipManagement/ShipManagement.csproj"
COPY . .
WORKDIR "/src/ShipManagement"
RUN dotnet build "ShipManagement.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ShipManagement.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ShipManagement.dll"]