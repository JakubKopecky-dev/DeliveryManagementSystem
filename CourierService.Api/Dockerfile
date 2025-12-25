# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy only csproj files first
COPY CourierService.Api/*.csproj CourierService.Api/
COPY CourierService.Application/*.csproj CourierService.Application/
COPY CourierService.Persistence/*.csproj CourierService.Persistence/
COPY CourierService.Domain/*.csproj CourierService.Domain/
RUN dotnet restore CourierService.Api/CourierService.Api.csproj

# Copy rest of the source code
COPY . .
WORKDIR /src/CourierService.Api
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet","CourierService.Api.dll"]