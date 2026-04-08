# Archetype Architecture Template

A **production-ready .NET 10 template** implementing **Archetype Architecture** with **CQRS**, comprehensive observability, and enterprise-grade infrastructure services.

[![.NET Version](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

---

## 📋 **Table of Contents**

- [Overview](#overview)
- [Features](#features)
- [Architecture](#architecture)
- [Getting Started](#getting-started)
- [Project Structure](#project-structure)
- [Key Technologies](#key-technologies)
- [Contributing](#contributing)
- [License](#license)

---

## 🎯 **Overview**

A **production-ready template** for building modern, scalable .NET APIs using **Archetype Architecture**. Features include:

- **CQRS pattern** with MediatR
- **Repository & Unit of Work** patterns
- **Message Bus** with MassTransit & RabbitMQ
- **Background Jobs** with Hangfire
- **Authentication** with Keycloak
- **Observability** with OpenTelemetry, Serilog, and Elastic APM
- **Caching** with Redis
- **Enterprise best practices** and clean architecture

---

## ✨ **Features**

### **Architecture & Patterns**
- Archetype Architecture - Features organized by capability
- CQRS - Command/Query separation
- Repository & Unit of Work patterns
- Outbox Pattern - Reliable message delivery
- Circuit Breaker - Resilience for external services

### **Security**
- Keycloak Integration - Enterprise IAM with JWT
- API Key Authentication
- Permission-Based Authorization
- CORS Configuration
- Data Encryption - AES/RSA providers

### **Observability**
- Structured Logging with Serilog
- Distributed Tracing with OpenTelemetry
- APM Integration with Elastic APM
- Correlation ID Tracking
- Health Checks - Kubernetes-ready
- Global Logging Filters for MediatR, MassTransit, Hangfire

### **Performance & Scalability**
- Redis Caching - Output & distributed cache
- Response Caching - HTTP-level caching
- Rate Limiting - API protection
- Pagination Support
- Async/Await throughout

### **Messaging & Background Jobs**
- MassTransit - Message bus abstraction
- RabbitMQ - Reliable message broker
- Outbox Pattern - Guaranteed delivery
- Hangfire - Recurring background jobs

### **Configuration**
- HashiCorp Vault - Secrets management
- Environment Variables - 12-factor compliance
- Type-Safe Options - Strongly-typed configuration

---

## 🏗️ **Architecture**

The template follows **Archetype Architecture** where each feature is organized as a Archetype containing:
- Endpoint/Controller
- Command/Query (CQRS)
- Handler
- Domain Logic
- Data Access

This promotes feature isolation and easier maintenance.

---

## 🚀 **Getting Started**

### Prerequisites
- .NET 10 SDK
- Docker (for dependent services)
- Visual Studio 2022+ or VS Code

### Quick Start

```bash
# Clone the repository
git clone https://github.com/yourusername/archetype-template.git
cd archetype-template

# Restore dependencies
dotnet restore

# Build
dotnet build

# Run tests
dotnet test

# Start the API
dotnet run --project Application/Services/Sample.Api
```

---

## 📁 **Project Structure**

```
├── Application/
│   ├── Services/
│   │   ├── Sample.Api/          # Main API service
│   │   └── Sample.Notification/ # Notification service
│   └── Modules/
│       └── Sample.Core.Api/     # Core business logic
├── Shared/
│   ├── Archetype.Infrastructure/  # Data access, messaging, caching
│   ├── Archetype.Shared/          # Shared utilities & extensions
│   ├── Archetype.Keycloak.Api/    # Keycloak integration
│   └── Archetype.Resources/       # Constants & resources
└── docs/                          # Documentation
```

---

## 🛠️ **Key Technologies**

- **.NET 10** - Latest .NET runtime
- **ASP.NET Core** - Web framework
- **EF Core** - Object-Relational Mapping
- **MediatR** - CQRS implementation
- **MassTransit** - Message bus
- **RabbitMQ** - Message broker
- **Redis** - Caching layer
- **Hangfire** - Background jobs
- **Keycloak** - Identity & Access Management
- **OpenTelemetry** - Distributed tracing
- **Serilog** - Structured logging
- **xUnit** - Testing framework

---

## 📚 **Documentation**

For detailed guides, see the `docs/` folder:
- Architecture deep-dive
- Configuration guide
- Deployment instructions
- API documentation

---

## 🤝 **Contributing**

Contributions are welcome! Please follow these steps:

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

---

## 📄 **License**

This project is licensed under the MIT License - see the `LICENSE` file for details.

---

## 📧 **Support**

For questions and support, please open an issue on GitHub or contact the maintainers.
