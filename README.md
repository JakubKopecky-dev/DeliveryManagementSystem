# DeliveryManagmentSystem

**DeliveryManagmentSystem** is a backend system built with a microservices architecture, applying **CQRS** and **event-driven communication**, where services communicate via **Kafka, gRPC, REST, and GraphQL**, with real-time updates delivered through **SignalR**.


## Tech Stack

### Back-end
- .NET 10
- REST, GraphQL
- SignalR
- gRPC
- MassTransit, Kafka
- EF Core
- ASP .NET Core Identity
- JWT (ES256), OIDC
- GraphQL Fusion Gateway

### Infrastructure
- SQL
- Elasticsearch

## Project Structure

### CourierService
- Responsible for creating and managing courier

### DeliveryService.Command
- Responsible for creating and managing delivery

### DeliveryService.Query
- Responsible for read-only access to delivery data

### NotificationService
- Responsible for sending notifications to the user  
- Uses SignalR  

### RouteService
- Responsible for integration with the OpenRoute service
- Return distance and duration

### UserService
- Responsible for user management, authentication, JWT generation and refresh token rotation  
- Supports classic email/password login using ASP.NET Core Identity  
- Supports external authentication via **Google (OIDC)**  
- After successful Google login, the backend validates the Google ID Token and issues its own JWT token


### GraphQLGateway
- Acts as the single entry point for all GraphQL clients
- Composes multiple GraphQL subgraphs into a unified schema
- Resolves cross-service relationships (e.g. Delivery → Courier)

## Features
- Decoupled microservices communicating via gRPC and Kafka
- User management
- User registration and login (JWT authentication, OIDC)
- Secure authentication using JWT access tokens with refresh token rotation
- Delivery and Courier management
- Distance and duration calculation between courier and customer
- Real-time notifications when delivery is created and status changed (SignalR)
- Gateway for all GraphQL clients (single entry point, resolves cross-service relationships)


## Observability & Telemetry
- OpenTelemetry is enabled across all services for distributed tracing and diagnostics
- Request-scoped log buffering is applied in selected services (e.g. UserService, CourierService)
- Log buffering reduces log noise by flushing logs only on warnings or errors
- This approach preserves full execution context while keeping logs concise


## Testing with Openroute service
- You need to create [Openroute service](https://openrouteservice.org/) account for API KEY

## Running the Project Locally

### Requirments
- .NET 10 SDK
- Docker + Docker Compose
- SQL Server database
- Elasticsearch


### Steps

###
```bash
git clone https://github/JakubKopecky-dev/DeliveryManagmentSystem.git
cd DeliveryManagmentSystem
```

#### 2) Run the application using Docker Compose:
```bash
docker-compose up -d --build
```

#### 3) After starting, the services are available here:
| Service               | URL / Swagger UI / GraphQL UI |
|------------------------|------------------|
| GraphQLGateway         | [Nitro](http://localhost:7000/graphql) |
| UserService            | [Swagger](http://localhost:7014/swagger/index.html) |
| SQL Server             | `localhost,1433` (user: `sa`, password: `Delivery2025!`) |
| Kibana                 | [Kibana](http://localhost:5601/app/home#/) |
| Kafka UI               | [Kafka-UI](http://localhost:8085/) |

#### 4) Configuration
- Configurations are defined in `docker-compose.yml` for local development.



## 📬 Contact
**Jakub Kopecký**  
 [🌐 GitHub](https://github.com/JakubKopecky-dev)  
 [💼 LinkedIn](https://www.linkedin.com/in/jakub-kopeck%C3%BD-a5919b278/)  



