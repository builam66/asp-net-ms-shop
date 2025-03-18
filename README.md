# Builam66 ASP.NET Microservices Shop

## Overview

This project is a demonstration of a microservices-based architecture implemented using ASP.NET Core. The system includes multiple microservices that handle various aspects of an e-commerce platform, such as managing product catalogs, shopping baskets, discounts, and orders. Each microservice is self-contained, follows domain-driven design principles, and communicates with others through well-defined interfaces.

---

## Directory Structure

```
├── README.md                  # Project documentation
├── LICENSE                    # License for the project
├── .deepsource.toml           # Configuration file for code analysis
├── .dockerignore              # Docker ignore file
└── src/
    ├── docker-compose.override.yml    # Docker Compose override configuration
    ├── docker-compose.yml             # Docker Compose configuration
    ├── eshop-microservices.sln        # Solution file for the project
    ├── launchSettings.json            # Launch settings for the project
    ├── .dockerignore                  # Docker ignore file for the `src` folder
    ├── BuildingBlocks/                # Shared building blocks for the application
    │   ├── Behaviors/                 # Behaviors for cross-cutting concerns
    │   ├── CQRS/                      # Command and Query Responsibility Segregation (CQRS) interfaces
    │   └── Exceptions/                # Custom exception handling
    ├── Services/                      # Microservices implementations
        ├── Basket/                    # Basket microservice
        ├── Catalog/                   # Catalog microservice
        ├── Discount/                  # Discount microservice
        └── Ordering/                  # Ordering microservice
```

---

## Microservices

### 1. Basket Service
Located at `src/Services/Basket/Basket.API`, this service handles operations related to the shopping basket.

**Key Components:**
- **Endpoints:** `GetBasket`, `StoreBasket`, `DeleteBasket`
- **Data Layer:** Interfaces and implementations for managing basket data
- **Models:** `ShoppingCart`, `ShoppingCartItem`
- **Custom Exceptions:** `BasketNotFoundException`

---

### 2. Catalog Service
Located at `src/Services/Catalog/Catalog.API`, this service manages the product catalog.

**Key Components:**
- **Endpoints:** `GetProducts`, `GetProductsByCategory`, `GetProductById`, `CreateProduct`, `UpdateProduct`, `DeleteProduct`
- **Models:** `Product`
- **Data:** Catalog initialization scripts
- **Custom Exceptions:** `ProductNotFoundException`

---

### 3. Discount Service
Located at `src/Services/Discount/Discount.Grpc`, this service provides discount-related functionality using gRPC.

**Key Components:**
- **Proto Definitions:** `discount.proto`
- **Database Context:** `DiscountContext`
- **Models:** `Coupon`
- **Service Implementation:** `DiscountService`
- **Data Migrations:** For managing schema changes

---

### 4. Ordering Service
The ordering service is split into multiple projects to adhere to clean architecture principles:

- **API:** `src/Services/Ordering/Ordering.API`
- **Application Layer:** `src/Services/Ordering/Ordering.Application`
- **Domain Layer:** `src/Services/Ordering/Ordering.Domain`
- **Infrastructure Layer:** `src/Services/Ordering/Ordering.Infrastructure`

**Key Components:**
- **Domain Models:** `Order`, `OrderItem`, `Customer`
- **Value Objects:** `Address`, `Payment`
- **Enums:** `OrderStatus`
- **Custom Exceptions:** `DomainException`
- **Dependency Injection Configuration**

---

## Building Blocks

Shared components that are used across the microservices to promote reusability and consistency:

### Behaviors
- **LoggingBehavior.cs**: Middleware for logging requests and responses.
- **ValidationBehavior.cs**: Middleware for validating requests.

### CQRS Interfaces
- **ICommand** and **ICommandHandler**
- **IQuery** and **IQueryHandler**

### Exception Handling
- **BadRequestException**: Handles 400 errors.
- **NotFoundException**: Handles 404 errors.
- **InternalServerException**: Handles 500 errors.
- **CustomExceptionHandler**: Centralized exception handling middleware.

---

## Prerequisites

1. **Docker**: Install Docker and Docker Compose.
2. **.NET SDK**: Ensure the .NET SDK is installed.
3. **SQL Server**: A running instance of SQL Server for the database-dependent services.

---

## Getting Started

### 1. Clone the Repository
```
git clone https://github.com/builam66/asp-net-ms-shop.git
cd builam66-asp-net-ms-shop
```

### 2. Set a docker registry (optional)
If you don't have a docker registry, you can use the default one from Docker Hub. If you have a private registry, set the environment variable:
```
export DOCKER_REGISTRY=<your-docker-registry>
```

### 3. Build and Run with Docker

To start all the services, use Docker Compose:
```
docker-compose up --build
```

### 4. Access the Services
- Basket API: `http://localhost:5001`
- Catalog API: `http://localhost:5002`
- Discount gRPC: `http://localhost:5003`
- Ordering API: `http://localhost:5004`

---

## Development

- Use Visual Studio to open the solution file (`eshop-microservices.sln`).
- Each service has its own `launchSettings.json` to configure local debugging.
- Update Docker Compose override file for custom configurations.

---

## Testing

- Unit tests and integration tests can be added in respective microservices.
- Use the `xUnit` or `MSTest` frameworks for testing.

---

## Contributing

Contributions are welcome! Please fork the repository, create a feature branch, and submit a pull request.

---

## License

This project is licensed under the MIT License. See `LICENSE` for more details.

