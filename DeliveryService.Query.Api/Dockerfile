# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy only csproj files first
COPY DeliveryService.Query.Api/*.csproj DeliveryService.Query.Api/
COPY DeliveryService.Query.Application/*.csproj DeliveryService.Query.Application/
COPY DeliveryService.Query.Infrastructure/*.csproj DeliveryService.Query.Infrastructure/
COPY DeliveryService.Query.Domain/*.csproj DeliveryService.Query.Domain/
RUN dotnet restore DeliveryService.Query.Api/DeliveryService.Query.Api.csproj

# Copy rest of the source code
COPY . .
WORKDIR /src/DeliveryService.Query.Api
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet","DeliveryService.Query.Api.dll"]