# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy only csproj files first
COPY DeliveryService.Query.SyncWorker/*.csproj DeliveryService.Query.SyncWorker/
RUN dotnet restore DeliveryService.Query.SyncWorker/DeliveryService.Query.SyncWorker.csproj

# Copy rest of the source code
COPY . .
WORKDIR /src/DeliveryService.Query.SyncWorker
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet","DeliveryService.Query.SyncWorker.dll"]