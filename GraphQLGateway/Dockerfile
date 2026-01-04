# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy only csproj files first
COPY GraphQLGateway/*.csproj GraphQLGateway/
RUN dotnet restore GraphQLGateway/GraphQLGateway.csproj

# Copy rest of the source code
COPY . .
WORKDIR /src/GraphQLGateway
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet","GraphQLGateway.dll"]