# ?? Tasheer Vertical Slice Template

A **production-ready .NET 10 template** implementing **Vertical Slice Architecture** with **CQRS**, comprehensive observability, and enterprise-grade infrastructure services.

[![.NET Version](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![Build Status](https://img.shields.io/badge/build-passing-brightgreen.svg)](https://dev.azure.com/mabusall/Vertical%20Slice%20Template)

---

## ?? **Table of Contents**

- [Overview](#overview)
- [Features](#features)
- [Architecture](#architecture)
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
- [Project Structure](#project-structure)
- [Configuration](#configuration)
- [Key Technologies](#key-technologies)
- [Development](#development)
- [Monitoring & Observability](#monitoring--observability)
- [Contributing](#contributing)
- [License](#license)

---

## ?? **Overview**

Tasheer is a **company-wide template** for building modern, scalable .NET APIs using **Vertical Slice Architecture**. It provides a complete foundation with:

- **CQRS pattern** using MediatR
- **Repository & Unit of Work** patterns
- **Message Bus** with MassTransit & RabbitMQ
- **Background Jobs** with Hangfire
- **Authentication** with Keycloak
- **Observability** with OpenTelemetry, Serilog, and Elastic APM
- **Caching** with Redis
- **Best practices** and **clean architecture**

### **Who Is This For?**

- ?? **Enterprise teams** building scalable APIs
- ??ž?? **Developers** who want a production-ready starting point
- ??? **Architects** looking for proven patterns
- ?? **Startups** needing enterprise-grade infrastructure

---

## ? **Features**

### **??? Architecture & Patterns**
- ? **Vertical Slice Architecture** - Features organized by capability
- ? **CQRS** - Command/Query separation with MediatR
- ? **Repository Pattern** - Generic data access layer
- ? **Unit of Work** - Transaction management
- ? **Outbox Pattern** - Reliable message delivery
- ? **Circuit Breaker** - Resilience for external services

### **?? Security**
- ? **Keycloak Integration** - Enterprise IAM with JWT
- ? **API Key Authentication** - Simple token-based auth
- ? **Permission-Based Authorization** - Fine-grained access control
- ? **CORS Configuration** - Flexible cross-origin support
- ? **Data Encryption** - AES/RSA encryption providers
- ? **Sensitive Data Masking** - Automatic PII masking in logs

### **?? Observability & Monitoring**
- ? **Structured Logging** - Serilog with multiple sinks
- ? **Distributed Tracing** - OpenTelemetry integration
- ? **APM Integration** - Elastic APM for performance monitoring
- ? **Correlation ID Tracking** - End-to-end request tracing
- ? **Memory Tracking** - Per-request memory allocation monitoring
- ? **Health Checks** - Kubernetes-ready liveness/readiness probes
- ? **Global Logging Filters** - Automatic logging for MediatR, MassTransit, Hangfire

### **?? Performance & Scalability**
- ? **Redis Caching** - Output cache + distributed cache
- ? **Response Caching** - HTTP response caching
- ? **Rate Limiting** - Protect APIs from abuse
- ? **Pagination Support** - Efficient data retrieval
- ? **Async/Await** - Non-blocking I/O throughout

### **?? Messaging & Background Jobs**
- ? **MassTransit** - Message bus abstraction
- ? **RabbitMQ** - Reliable message broker
- ? **Outbox Pattern** - Guaranteed message delivery
- ? **Hangfire** - Recurring background jobs
- ? **Dynamic Job Registration** - Configuration-driven jobs

### **?? Notifications**
- ? **Email** - SMTP with Razor templates
- ? **Push Notifications** - Firebase Cloud Messaging (FCM)
- ? **Bilingual Support** - Arabic + English templates

### **?? Configuration Management**
- ? **HashiCorp Vault** - Secrets management
- ? **Environment Variables** - 12-factor app compliance
- ? **Per-Environment Config** - Development/Staging/Production
- ? **Type-Safe Options** - Strongly-typed configuration classes

### **??? Database**
- ? **Entity Framework Core** - Modern ORM
- ? **Migrations** - Database versioning
- ? **Audit Tracking** - Automatic Created/Modified tracking
- ? **Retry Policies** - Resilience for transient failures
- ? **Read/Write Separation** - Read-only repositories

---

## ??? **Architecture**

### **Vertical Slice Architecture**

Each feature is a self-contained vertical slice:

```
Feature (e.g., CreateOrder)
??? Command/Query         (Request model)
??? Handler               (Business logic)
??? Validation            (FluentValidation rules)
??? Endpoint              (API endpoint)
```

**Benefits:**
- ?? High cohesion - all related code together
- ?? Low coupling - features don't depend on each other
- ?? Easy to test - isolated feature logic
- ?? Easy to modify - changes don't ripple across codebase

### **Layered Structure**

```
???????????????????????????????????????
?   Application Layer                 ?
?   (API Endpoints, Modules)          ?
???????????????????????????????????????
?   Infrastructure Layer              ?
?   (CQRS, Repository, Auth, etc.)    ?
???????????????????????????????????????
?   Shared Layer                      ?
?   (Common utilities, models)        ?
???????????????????????????????????????
```

---

## ?? **Prerequisites**

### **Required:**
- ? [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- ? [SQL Server 2019+](https://www.microsoft.com/sql-server/sql-server-downloads) (or SQL Server Express)
- ? [Visual Studio 2022 17.13+](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)

### **Optional (for full features):**
- ?? [RabbitMQ](https://www.rabbitmq.com/download.html) - Message bus (can disable in config)
- ?? [Redis](https://redis.io/download) - Caching (can disable in config)
- ?? [Keycloak](https://www.keycloak.org/downloads) - Authentication (required for protected endpoints)
- ?? [Elasticsearch](https://www.elastic.co/downloads/elasticsearch) - Logging (optional)
- ?? [Kibana](https://www.elastic.co/downloads/kibana) - Log visualization (optional)
- ?? [HashiCorp Vault](https://www.vaultproject.io/downloads) - Secrets (optional)

### **Docker Alternative:**
All dependencies can be run via Docker (see [Docker Setup](#docker-setup) section).

---

## ?? **Getting Started**

### **1. Clone the Repository**

```bash
git clone https://dev.azure.com/mabusall/Vertical%20Slice%20Template/_git/Vertical%20Slice%20Template
cd "Vertical Slice Template"
```

### **2. Configure User Secrets**

The template uses **User Secrets** for sensitive configuration (development only).

```bash
# Navigate to the API project
cd "Application\Services\Tasheer.Api"

# Initialize user secrets
dotnet user-secrets init

# Set the user secrets file
# Copy from: %APPDATA%\Microsoft\UserSecrets\19e955c2-952e-4df9-8e71-9fd0d177a48c\secrets.json
```

**Minimal Configuration (Required):**

```json
{
  "Application": {
    "ApiKey": "YOUR-API-KEY-HERE",
    "PrivateKey": "YOUR-PRIVATE-KEY",
    "PublicKey": "YOUR-PUBLIC-KEY",
    "SupportedLanguages": ["ar", "en"],
    "DbConnection": "Server=.;Database=SampleDB;Trusted_Connection=true;TrustServerCertificate=true"
  },
  "Cors": {
    "IsEnabled": true,
    "PolicyName": "DevelopmentPolicy",
    "AllowAnyOrigin": true,
    "AllowCredentials": false
  }
}
```

> ?? **Note:** See `appsettings.Cors.example.json` for full configuration examples.

### **3. Setup Database**

```bash
# Create database and apply migrations
dotnet ef database update --project "Application\Modules\Tasheer.Core.Api" --startup-project "Application\Services\Tasheer.Api"

# Apply Outbox migrations (for MassTransit)
dotnet ef database update --context OutBoxDbContext --project "Shared\Tasheer.Infrastructure" --startup-project "Application\Services\Tasheer.Api"
```

### **4. Run the Application**

```bash
# Navigate to API project
cd "Application\Services\Tasheer.Api"

# Run the application
dotnet run
```

**API will be available at:**
- ?? HTTP: `http://localhost:5000`
- ?? HTTPS: `https://localhost:5001`
- ?? API Docs: `https://localhost:5001/scalar/v1`
- ?? Health Check: `https://localhost:5001/health`
- ?? Ping: `https://localhost:5001/ping`

### **5. Test the API**

```bash
# Test with curl (use your API key from secrets)
curl -X GET "https://localhost:5001/order" \
  -H "x-api-key: YOUR-API-KEY-HERE" \
  -k
```

Or open **Scalar UI** at `https://localhost:5001/scalar/v1` for interactive API documentation.

---

## ?? **Project Structure**

```
Vertical Slice Template/
?
??? Application/
?   ??? Services/
?   ?   ??? Tasheer.Api              # ?? Main API Entry Point
?   ?   ??? Tasheer.Notification     # ?? Notification Service
?   ?
?   ??? Modules/
?       ??? Tasheer.Core.Api         # ?? Business Module (Orders example)
?       ??? Keycloak.Api             # ?? Authentication Module
?
??? Shared/
?   ??? Tasheer.Infrastructure       # ? Core Infrastructure Services
?   ??? Tasheer.Shared               # ?? Common Utilities & Models
?   ??? Tasheer.Resources            # ?? Localization Resources
?
??? docs/                            # ?? Documentation
??? AUDIT-REPORT.md                  # ?? Solution Audit Report
??? README.md                        # ?? This file
```

### **Key Projects**

| Project | Purpose | Type |
|---------|---------|------|
| **Tasheer.Infrastructure** | Core infrastructure (CQRS, Repository, Auth, etc.) | Shared |
| **Tasheer.Shared** | Common utilities and configuration models | Shared |
| **Tasheer.Api** | Main API host and entry point | Service |
| **Tasheer.Core.Api** | Business logic module (Orders feature) | Module |
| **Keycloak.Api** | Authentication & user management | Module |
| **Tasheer.Notification** | Email & push notifications | Service |

---

## ?? **Configuration**

### **Configuration Sources (Priority Order)**

1. **User Secrets** (Development only)
2. **HashiCorp Vault** (if enabled)
3. **Environment Variables** (prefix: `FlixHubKeys:`)
4. **appsettings.{Environment}.json**
5. **appsettings.json**

### **Core Configuration Sections**

#### **Application Settings**
```json
{
  "Application": {
    "ApiKey": "YOUR-API-KEY",
    "PrivateKey": "encryption-key",
    "PublicKey": "encryption-key",
    "SupportedLanguages": ["ar", "en"],
    "DbConnection": "Server=.;Database=SampleDB;Integrated Security=true"
  }
}
```

#### **CORS**
```json
{
  "Cors": {
    "IsEnabled": true,
    "PolicyName": "DefaultCorsPolicy",
    "AllowAnyOrigin": false,
    "AllowedOrigins": ["https://app.yourcompany.com"],
    "AllowCredentials": true
  }
}
```

#### **Logging & Monitoring**
```json
{
  "LoggingAndMonitoring": {
    "IsEnabled": true,
    "Environment": "Development",
    "Logging": {
      "Project": "Tasheer",
      "Application": "Tasheer.Api",
      "FolderPath": "C:\\Logs\\Tasheer",
      "EnableConsoleLogs": true,
      "EnableFileLogs": true
    },
    "Elasticsearch": {
      "IsEnabled": true,
      "AddressUri": "http://localhost:9200"
    },
    "Apm": {
      "AddressUri": "http://localhost:8201",
      "ServiceName": "Tasheer.Api",
      "ServiceVersion": "1.0.0"
    },
    "OpenTelemetry": {
      "AddressUri": "http://localhost:8201",
      "EnableTracing": true,
      "EnableMetrics": true
    }
  }
}
```

#### **Message Bus (RabbitMQ)**
```json
{
  "MessageBus": {
    "AddressUri": "rabbitmq://localhost:5672",
    "AmqpAddressUri": "amqp://localhost:5672",
    "IsEnabled": true,
    "UserName": "guest",
    "Password": "guest",
    "Prefix": "tasheer-",
    "Consumers": {
      "EmailEndpoint": {
        "Consumer": "Tasheer.Notification.EventHandlers.Handlers.SendEmailConsumer, Tasheer.Notification",
        "IsActive": true,
        "UseDatabase": true
      }
    }
  }
}
```

#### **Background Jobs (Hangfire)**
```json
{
  "Hangfire": {
    "IsEnabled": true,
    "SchemaName": "Hangfire",
    "Prefix": "tasheer-",
    "Tasks": {
      "CacheOrdersJob": {
        "IsEnabled": true,
        "AutoStart": true,
        "Handler": "Tasheer.Core.Api.Jobs.CacheOrdersJob, Tasheer.Core.Api",
        "Schedule": "0.00:05:00"
      }
    }
  }
}
```

#### **Redis**
```json
{
  "Redis": {
    "AddressUri": "localhost:6379",
    "IsEnabled": true,
    "SecureSsl": false,
    "Password": "",
    "OutputCachePrefix": "TasheerOutputCache_",
    "DistributedCachePrefix": "TasheerDistributedCache_"
  }
}
```

#### **Keycloak**
```json
{
  "Keycloak": {
    "AddressUri": "http://localhost:8080",
    "Realms": {
      "Custom": {
        "Name": "tasheer",
        "Clients": {
          "TasheerCli": {
            "ClientId": "tasheer-cli",
            "Secret": "your-client-secret"
          }
        }
      }
    }
  }
}
```

> ?? **Full Configuration Reference:** See `CONFIGURATION.md` (coming soon)

---

## ??? **Architecture**

### **Vertical Slice Pattern**

Each feature is self-contained:

```
Features/Orders/
??? Create.Command.cs       # Request model
??? Create.Handler.cs       # Business logic
??? Create.Validation.cs    # FluentValidation rules
??? Delete.Command.cs
??? Delete.Handler.cs
??? GetList.Query.cs
??? GetList.Handler.cs
??? Endpoints/
    ??? Order.cs            # API endpoints
```

### **CQRS Flow**

```
HTTP Request
  ?
API Endpoint (Minimal API)
  ?
IMediator.Send(Command/Query)
  ?
ValidationBehavior ? Validate request
  ?
LoggingBehavior ? Log + trace
  ?
Handler ? Execute business logic
  ?
Repository ? Data access
  ?
UnitOfWork.SaveChanges ? Persist
  ?
Response
```

### **Message Bus Flow (Outbox Pattern)**

```
HTTP Request ? Handler
  ?
IBusService.Publish(Event, IsOutbox = true)
  ?
MassTransit Outbox (writes to DB)
  ?
UnitOfWork.SaveChanges (atomically commits event + data)
  ?
MassTransit Delivery Service (reads from outbox)
  ?
RabbitMQ (publishes message)
  ?
Consumer (processes message)
```

### **Correlation ID Propagation**

```
HTTP Request
  ? CorrelationIdMiddleware (generates/reads ID)
  ? LoggingBehavior (MediatR) - logs with ID
  ? BusService (MassTransit) - propagates in message headers
  ? LoggingConsumeFilter (Consumer) - logs with ID
  ? HangfireLoggingFilter (Job) - logs with ID

Result: Same correlation ID across entire distributed flow!
```

---

## ?? **Key Technologies**

### **Core**
- [.NET 10](https://dotnet.microsoft.com/) - Latest .NET runtime
- [ASP.NET Core Minimal APIs](https://learn.microsoft.com/aspnet/core/fundamentals/minimal-apis) - Lightweight endpoints
- [Entity Framework Core](https://learn.microsoft.com/ef/core/) - ORM and data access
- [MediatR](https://github.com/jbogard/MediatR) - CQRS implementation

### **Messaging & Jobs**
- [MassTransit](https://masstransit.io/) - Message bus abstraction
- [RabbitMQ](https://www.rabbitmq.com/) - Message broker
- [Hangfire](https://www.hangfire.io/) - Background job scheduler

### **Authentication & Security**
- [Keycloak](https://www.keycloak.org/) - Identity and Access Management
- [JWT Bearer](https://jwt.io/) - Token-based authentication

### **Observability**
- [Serilog](https://serilog.net/) - Structured logging
- [OpenTelemetry](https://opentelemetry.io/) - Distributed tracing & metrics
- [Elastic APM](https://www.elastic.co/apm) - Application Performance Monitoring

### **Caching**
- [Redis](https://redis.io/) - Distributed caching
- [StackExchange.Redis](https://github.com/StackExchange/StackExchange.Redis) - .NET client

### **Validation & Mapping**
- [FluentValidation](https://fluentvalidation.net/) - Validation rules
- [Mapster](https://github.com/MapsterMapper/Mapster) - Object mapping

### **Resilience**
- [Polly v8](https://www.pollydocs.org/) - Retry, circuit breaker, timeout policies

### **Secrets Management**
- [HashiCorp Vault](https://www.vaultproject.io/) - Centralized secrets

### **Notifications**
- [Firebase Cloud Messaging](https://firebase.google.com/docs/cloud-messaging) - Push notifications
- [Razor Pages](https://learn.microsoft.com/aspnet/core/razor-pages/) - Email templates

---

## ?? **Development**

### **Adding a New Feature (Vertical Slice)**

**Example:** Add "Update Order" feature

#### **Step 1: Create Command**
```csharp
// Features/Orders/Update.Command.cs
public record UpdateOrderCommand(int Id, decimal Price) : ICommand<Unit>;
```

#### **Step 2: Create Handler**
```csharp
// Features/Orders/Update.Handler.cs
internal class UpdateOrderCommandHandler(ISampleDbUnitOfWork uow)
    : ICommandHandler<UpdateOrderCommand, Unit>
{
    public async Task<Unit> Handle(UpdateOrderCommand command, CancellationToken ct)
    {
        var order = await uow.OrdersRepository.GetByIdAsync(command.Id, ct)
            ?? throw new NotFoundException("Order not found");
        
        order.Price = command.Price;
        order.Vat = command.Price * 0.15m;
        order.TotalPrice = order.Price + order.Vat;
        
        await uow.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
```

#### **Step 3: Create Validation**
```csharp
// Features/Orders/Update.Validation.cs
internal class UpdateOrderCommandValidator : AbstractValidator<UpdateOrderCommand>
{
    public UpdateOrderCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Price).GreaterThan(0);
    }
}
```

#### **Step 4: Add Endpoint**
```csharp
// Features/Orders/Endpoints/Order.cs
group.MapPut("/{id:int}", async (int id, [FromBody] UpdateOrderDto dto,
                                  ISender sender, CancellationToken ct) =>
{
    await sender.Send(new UpdateOrderCommand(id, dto.Price), ct);
    return Results.NoContent();
})
.WithName("UpdateOrder")
.RequireApiKey();
```

**Done!** ? Validation, logging, tracing, correlation ID all automatic!

---

### **Adding a Background Job**

#### **Step 1: Create Job Class**
```csharp
// Jobs/SendReportJob.cs
internal class SendReportJob(ISampleDbUnitOfWork uow, ILogger<SendReportJob> logger)
    : IHangfireJob
{
    public async Task ExecuteAsync()
    {
        // Your job logic
        var orders = await uow.OrdersRepository.GetListAsync(o => true, CancellationToken.None);
        logger.LogInformation("Found {Count} orders", orders.Count);
    }
}
```

#### **Step 2: Register in Configuration**
```json
{
  "Hangfire": {
    "Tasks": {
      "SendReportJob": {
        "IsEnabled": true,
        "AutoStart": false,
        "Handler": "Tasheer.Core.Api.Jobs.SendReportJob, Tasheer.Core.Api",
        "Schedule": "0.01:00:00"
      }
    }
  }
}
```

**Done!** ? Job runs every hour with automatic logging!

---

### **Publishing a Message**

```csharp
// In your handler
public class CreateOrderCommandHandler(ISampleDbUnitOfWork uow, IBusService bus)
    : ICommandHandler<CreateOrderCommand, CreateOrderResult>
{
    public async Task<CreateOrderResult> Handle(CreateOrderCommand command, CancellationToken ct)
    {
        var order = command.Adapt<Order>();
        uow.OrdersRepository.Insert(order);
        
        // Publish event with outbox pattern (guaranteed delivery)
        await bus.Publish(new OrderCreatedEvent 
        { 
            OrderId = order.Id,
            IsOutbox = true  // Uses outbox pattern
        }, ct);
        
        await uow.SaveChangesAsync(ct);
        
        return new CreateOrderResult(order.Uuid);
    }
}
```

---

## ?? **Monitoring & Observability**

### **Accessing Monitoring Tools**

| Tool | URL | Purpose |
|------|-----|---------|
| **API Documentation** | `http://localhost:5000/scalar/v1` | Interactive API docs |
| **Hangfire Dashboard** | `http://localhost:5000/hangfire` | Background jobs |
| **Health Checks** | `http://localhost:5000/health` | Overall health |
| **Readiness Probe** | `http://localhost:5000/health/ready` | K8s readiness |
| **Liveness Probe** | `http://localhost:5000/health/live` | K8s liveness |
| **Ping Endpoint** | `http://localhost:5000/ping` | Quick check |
| **Kibana** | `http://localhost:5601` | Logs & APM |
| **Elasticsearch** | `http://localhost:9200` | Log storage |

### **Viewing Logs**

#### **Console (Development)**
```bash
dotnet run

# Output:
[START] CreateOrderCommand [UserId: user-123]
[END] CreateOrderCommand handled in 0.25s, Memory: 2508.80 KB
```

#### **Elasticsearch (Development/Production)**
Open Kibana: `http://localhost:5601`
```
Discover ? Search: "SystemCorrelationId: abc-123"
Result: All logs for that request (HTTP ? MediatR ? MassTransit ? Hangfire)
```

#### **Elastic APM (Performance)**
```
APM ? Services ? Tasheer.Api ? Transactions ? GET /order
View:
  - Duration breakdown
  - Database queries
  - Memory allocation
  - GC collections
  - Correlation ID
```

### **Correlation ID Tracking**

Every log entry includes `SystemCorrelationId`:

```json
{
  "@timestamp": "2024-01-15T10:23:45Z",
  "message": "[END] GetListOrderQuery handled in 0.25s",
  "SystemCorrelationId": "4258a3f1-dbe6-49bc-b0d7-e62f6388886a",
  "TraceId": "4bf92f3577b34da6a3ce929d0e0e4736",
  "SpanId": "00f067aa0ba902b7"
}
```

**Use Cases:**
- ?? Trace request through entire system
- ?? Debug issues across distributed services
- ?? Analyze performance bottlenecks
- ?? Correlate errors with root cause

---

## ?? **Docker Setup**

> ?? **Note:** Docker configuration coming soon! See [AUDIT-REPORT.md](AUDIT-REPORT.md) for roadmap.

### **Quick Start (All Dependencies)**

```bash
# Coming soon: docker-compose.yml
docker-compose up -d

# Will start:
# - SQL Server
# - RabbitMQ
# - Redis
# - Elasticsearch
# - Kibana
# - Keycloak
```

---

## ?? **Testing**

> ?? **Note:** Test projects coming soon! See [AUDIT-REPORT.md](AUDIT-REPORT.md).

### **Planned Test Projects:**
- `Tasheer.Infrastructure.Tests` - Unit tests for infrastructure
- `Tasheer.Core.Api.Tests` - Unit tests for business logic
- `Tasheer.Api.IntegrationTests` - End-to-end API tests

---

## ?? **Documentation**

### **Available Documentation**
- ? [AUDIT-REPORT.md](AUDIT-REPORT.md) - Solution audit and improvement roadmap
- ? [appsettings.Cors.example.json](Application/Services/Tasheer.Api/appsettings.Cors.example.json) - CORS configuration examples

### **Coming Soon**
- ?? ARCHITECTURE.md - Detailed architecture guide
- ?? SETUP.md - Step-by-step setup instructions
- ?? DOCKER.md - Docker deployment guide
- ?? KUBERNETES.md - Kubernetes deployment guide
- ?? SECURITY.md - Security best practices
- ?? DEPLOYMENT.md - Production deployment guide

---

## ??? **Development Guidelines**

### **Code Organization**
- ? Use **Vertical Slice** pattern - group by feature, not layer
- ? Keep features **independent** - avoid cross-feature dependencies
- ? Use **CQRS** - separate commands from queries
- ? Apply **FluentValidation** - validate all commands
- ? Follow **naming conventions** - Command, Handler, Validation suffixes

### **Database**
- ? Always use **repository pattern** - don't access DbContext directly
- ? Use **Unit of Work** - manage transactions
- ? Create **migrations** for schema changes
- ? Use **AsNoTracking** for read-only queries
- ? **Project to DTOs** - don't return entities from repositories

### **Logging**
- ? **Don't log manually** - LoggingBehavior handles it
- ? Use **structured logging** - pass parameters, not formatted strings
- ? Log at **appropriate levels** - Debug, Info, Warning, Error
- ? **Mask sensitive data** - passwords, tokens, PII

### **API Design**
- ? Use **HTTP status codes** correctly (200, 201, 400, 404, 500)
- ? Return **ProblemDetails** for errors (automatic via CustomExceptionHandler)
- ? Use **route constraints** - `{id:int}`, `{uuid:guid}`
- ? Apply **rate limiting** - protect expensive endpoints
- ? Add **API documentation** - WithSummary, WithDescription

---

## ?? **Best Practices Implemented**

### **? Correlation ID Tracking**
Every request gets a unique ID that flows through:
- HTTP request/response headers
- All log entries
- MediatR commands/queries
- MassTransit messages
- Hangfire jobs
- OpenTelemetry spans
- Elastic APM transactions

### **? Global Logging Filters**
Automatic logging for:
- **MediatR** - All commands/queries logged with timing
- **MassTransit** - All consumers logged with timing
- **Hangfire** - All jobs logged with timing

### **? Memory Tracking**
Every request tracked for:
- Memory allocation (MB/KB/Bytes)
- GC collections (Gen0/Gen1/Gen2)
- Performance warnings (>10 MB)

### **? Exception Handling**
- ProblemDetails for all errors
- Correlation ID included
- Validation errors detailed
- OpenTelemetry span tagging
- Structured logging

### **? Configuration-Driven**
Enable/disable features without code changes:
- Redis caching
- RabbitMQ messaging
- Hangfire jobs
- CORS policies
- Logging sinks
- HashiCorp Vault

---

## ?? **Security**

### **Authentication**
- **Keycloak JWT** - Production authentication
- **API Key** - Simple authentication for services

### **Authorization**
```csharp
// Endpoint-level authorization
.RequireAuthorization()
.RequireApiKey()
.RequirePermissions(OrderPermissions.Create, OrderPermissions.Update)
```

### **Data Protection**
- Encryption providers (AES, RSA, File)
- Sensitive data masking in logs
- HTTPS enforcement

---

## ?? **Common Tasks**

### **Update Database Schema**
```bash
# Add migration
dotnet ef migrations add YourMigrationName --project "Application\Modules\Tasheer.Core.Api"

# Apply migration
dotnet ef database update --project "Application\Modules\Tasheer.Core.Api" --startup-project "Application\Services\Tasheer.Api"
```

### **View Hangfire Dashboard**
```bash
# Run app
dotnet run

# Open browser
http://localhost:5000/hangfire
```

### **Trigger a Job Manually**
```bash
# In Hangfire dashboard, click "Trigger Now" button
# Or via code:
RecurringJob.TriggerJob("tasheer-cacheordersjob");
```

### **Check Health Status**
```bash
curl http://localhost:5000/health
```

**Response:**
```json
{
  "status": "Healthy",
  "totalDuration": "00:00:00.0450000",
  "entries": {
    "redis": { "status": "Healthy" },
    "rabbitmq": { "status": "Healthy" },
    "sqlserver": { "status": "Healthy" }
  }
}
```

---

## ?? **Troubleshooting**

### **Application won't start**

**Check:**
1. SQL Server is running
2. Connection string is correct
3. User secrets are configured
4. All required config sections present

**View startup errors:**
```bash
dotnet run --verbosity detailed
```

### **CORS errors in browser**

**Solution:**
```json
{
  "Cors": {
    "AllowAnyOrigin": true,  // Development only!
    "AllowCredentials": false
  }
}
```

### **RabbitMQ connection failed**

**Option 1:** Disable RabbitMQ temporarily
```json
{
  "MessageBus": {
    "IsEnabled": false
  }
}
```

**Option 2:** Run RabbitMQ via Docker
```bash
docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:management
```

### **Redis connection failed**

**Option 1:** Disable Redis temporarily
```json
{
  "Redis": {
    "IsEnabled": false
  }
}
```

**Option 2:** Run Redis via Docker
```bash
docker run -d --name redis -p 6379:6379 redis:latest
```

---

## ?? **Learning Resources**

### **Patterns & Architecture**
- [Vertical Slice Architecture](https://www.jimmybogard.com/vertical-slice-architecture/)
- [CQRS Pattern](https://learn.microsoft.com/azure/architecture/patterns/cqrs)
- [Repository Pattern](https://learn.microsoft.com/aspnet/mvc/overview/older-versions/getting-started-with-ef-5-using-mvc-4/implementing-the-repository-and-unit-of-work-patterns-in-an-asp-net-mvc-application)
- [Outbox Pattern](https://microservices.io/patterns/data/transactional-outbox.html)

### **Technologies**
- [MediatR Documentation](https://github.com/jbogard/MediatR/wiki)
- [MassTransit Documentation](https://masstransit.io/documentation/concepts)
- [Hangfire Documentation](https://docs.hangfire.io/)
- [Keycloak Documentation](https://www.keycloak.org/documentation)
- [OpenTelemetry .NET](https://opentelemetry.io/docs/languages/net/)

---

## ?? **Contributing**

### **Development Workflow**

1. **Create a feature branch**
   ```bash
   git checkout -b feature/your-feature-name
   ```

2. **Make changes following patterns**
   - Add features using Vertical Slice pattern
   - Include validation with FluentValidation
   - Update migrations if schema changes
   - Follow existing naming conventions

3. **Test your changes**
   ```bash
   dotnet build
   dotnet run
   # Test endpoints manually or with tests
   ```

4. **Commit and push**
   ```bash
   git add .
   git commit -m "Add: Your feature description"
   git push origin feature/your-feature-name
   ```

5. **Create Pull Request**
   - Target: `main` or `develop` branch
   - Include description of changes
   - Reference any related work items

---

## ?? **License**

This project is licensed under the **MIT License** - see the [LICENSE](LICENSE) file for details.

---

## ?? **Support**

### **Internal Support**
- ?? **Email:** platform-team@yourcompany.com
- ?? **Teams:** #platform-support channel
- ?? **Wiki:** [Internal Documentation](https://wiki.yourcompany.com/tasheer)

### **Issues**
For bugs or feature requests, create an issue in [Azure DevOps](https://dev.azure.com/mabusall/Vertical%20Slice%20Template/_workitems).

---

## ?? **Credits**

**Developed by:** Platform Team  
**Architecture:** Mohannad Abu Sall  
**Special Thanks:** Mohammed Al Turaif (Correlation ID middleware fixes)

---

## ?? **Highlights**

### **Why Use This Template?**

? **Production-Ready** - Battle-tested patterns and infrastructure  
? **Observable** - World-class logging, tracing, and monitoring  
? **Scalable** - Message bus, background jobs, caching  
? **Secure** - Keycloak, API keys, encryption  
? **Flexible** - Configuration-driven, easy to customize  
? **Modern** - .NET 10, latest best practices  
? **Well-Architected** - Clean, maintainable code  

### **Perfect For:**
- ?? Microservices architecture
- ?? Mobile app backends
- ?? Web application APIs
- ?? Internal service APIs
- ?? Data processing services

---

## ?? **Quick Links**

- ?? [Audit Report](AUDIT-REPORT.md) - What's missing and roadmap
- ??? [Architecture Guide](docs/ARCHITECTURE.md) _(coming soon)_
- ?? [Setup Guide](docs/SETUP.md) _(coming soon)_
- ?? [Docker Guide](docs/DOCKER.md) _(coming soon)_
- ?? [Kubernetes Guide](docs/KUBERNETES.md) _(coming soon)_

---

## ?? **Roadmap**

### **v1.0 (Current)**
- ? Vertical Slice Architecture
- ? CQRS with MediatR
- ? Complete observability stack
- ? Message bus with outbox pattern
- ? Background jobs
- ? Redis caching
- ? Keycloak authentication

### **v1.1 (Next)**
- ?? API Versioning
- ?? Docker support
- ?? Response compression
- ?? Security headers
- ?? Database indexes
- ?? CI/CD pipeline

### **v1.2 (Future)**
- ?? Kubernetes manifests
- ?? Soft delete pattern
- ?? Idempotency keys
- ?? Feature flags
- ?? GraphQL endpoint
- ?? gRPC services

---

**Built with ?? by the Platform Team**

---

_Last Updated: 2024_
