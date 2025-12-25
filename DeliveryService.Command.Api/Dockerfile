# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy only csproj files first
COPY DeliveryService.Command.Api/*.csproj DeliveryService.Command.Api/
COPY DeliveryService.Command.Application/*.csproj DeliveryService.Command.Application/
COPY DeliveryService.Command.Persistence/*.csproj DeliveryService.Command.Persistence/
COPY DeliveryService.Command.Domain/*.csproj DeliveryService.Command.Domain/
RUN dotnet restore DeliveryService.Command.Api/DeliveryService.Command.Api.csproj

# Copy rest of the source code
COPY . .
WORKDIR /src/DeliveryService.Command.Api
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet","DeliveryService.Command.Api.dll"]