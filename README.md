# 🏥 Pharmacy Inventory & Prescription Management System

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4)](https://dotnet.microsoft.com/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-13+-336791)](https://www.postgresql.org/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![Build Status](https://img.shields.io/badge/build-passing-brightgreen.svg)](#)

## 📋 Overview

A secure, role-based web API system for managing prescriptions and pharmacy inventory. Built with modern C# practices,
featuring JWT authentication, comprehensive role management, and efficient inventory tracking.

### 🎯 Key Features

- **🔐 Secure Authentication**: JWT-based authentication with role-based access control
- **👨‍⚕️ Doctor Portal**: Create and manage digital prescriptions
- **💊 Pharmacy Management**: Comprehensive inventory tracking and management
- **👥 Patient Access**: View prescription history and status
- **📊 Analytics**: Track usage metrics and system performance
- **🏗️ Modern Architecture**: Clean architecture with DDD patterns

### 👥 User Roles

| Role           | Permissions                                                |
|----------------|------------------------------------------------------------|
| **Doctor**     | Create prescriptions, view own patients                    |
| **Pharmacist** | Manage inventory, fulfill prescriptions, view stock alerts |
| **Patient**    | View prescription history and status                       |
| **Admin**      | User management, system administration                     |

## 🏗️ Architecture

```
src/
├── SolveStation.Common/           # Shared models, exceptions, interfaces
├── SolveStation.Authentication/   # JWT, security, authorization
├── SolveStation.UserManagement/   # User CRUD, role management
├── SolveStation.PrescriptionManagement/ # Prescription workflows
├── SolveStation.InventoryManagement/    # Stock management, alerts
├── SolveStation.Data/            # Entity Framework, repositories
└── SolveStation.PharmacyApi/     # Web API, controllers, middleware
```

## 🚀 Quick Start

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL 13+](https://www.postgresql.org/download/)
- [Git](https://git-scm.com/downloads)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)

### 🔧 Development Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/SolveStation/solve-inventory-pharmacy.git
   cd solve-inventory-pharmacy
   ```

2. **Set up database**
   ```bash
   # Start PostgreSQL with Docker
   docker-compose up -d postgres
   
   # Or install PostgreSQL locally and create database
   createdb pharmacy_inventory
   ```

3. **Configure application settings**
   ```bash
   # Copy example settings
   cp src/SolveStation.PharmacyApi/appsettings.example.json src/SolveStation.PharmacyApi/appsettings.json
   
   # Update connection string and JWT settings
   ```

4. **Install dependencies and build**
   ```bash
   dotnet restore
   dotnet build
   ```

5. **Run database migrations**
   ```bash
   dotnet ef database update -p src/SolveStation.Data -s src/SolveStation.PharmacyApi
   ```

6. **Start the application**
   ```bash
   dotnet run --project src/SolveStation.PharmacyApi
   ```

7. **Access the application**
    - API: https://localhost:5001
    - Swagger UI: https://localhost:5001/swagger
    - Health Check: https://localhost:5001/health

### 🐳 Docker Development

```bash
# Start all services
docker-compose up -d

# View logs
docker-compose logs -f api

# Stop services
docker-compose down
```

## 🧪 Testing

### Run Tests

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test project
dotnet test tests/SolveStation.UserManagement.Tests
```

### Test Structure

- **Unit Tests**: Service and controller logic testing
- **Integration Tests**: API endpoint testing with test database
- **Performance Tests**: Load testing and benchmarks

## 📖 API Documentation

### Authentication Endpoints

```
POST /api/auth/login          # User login
POST /api/auth/refresh        # Token refresh
POST /api/auth/logout         # User logout
```

### User Management

```
GET    /api/users             # List users (Admin only)
POST   /api/users             # Create user (Admin only)
GET    /api/users/{id}        # Get user details
PUT    /api/users/{id}        # Update user
DELETE /api/users/{id}        # Delete user (Admin only)
```

### Prescriptions

```
GET    /api/prescriptions     # List prescriptions (role-based)
POST   /api/prescriptions     # Create prescription (Doctor only)
GET    /api/prescriptions/{id} # Get prescription details
PUT    /api/prescriptions/{id} # Update prescription status
```

### Inventory

```
GET    /api/inventory         # List inventory (Pharmacist only)
POST   /api/inventory         # Add inventory item
PUT    /api/inventory/{id}    # Update inventory
GET    /api/inventory/alerts  # Low stock alerts
```

For detailed API documentation, visit `/swagger` when running the application.

## 🤝 Contributing

We welcome contributions! Please see our [Contributing Guidelines](CONTRIBUTING.md) for details.

### Development Workflow

1. **Create feature branch**: `git checkout -b feature/YOUR-MODULE-001-description`
2. **Follow coding conventions**: See [Team Task Delegation](docs/TEAM_TASK_DELEGATION.md)
3. **Write tests**: Maintain >80% code coverage
4. **Update documentation**: Keep README and API docs current
5. **Submit Pull Request**: Include description and link issues

### Code Quality

- **StyleCop**: Enforces coding standards
- **Nullable Reference Types**: Enabled project-wide
- **EditorConfig**: Consistent formatting
- **SonarQube**: Code quality analysis

## 🏗️ Project Structure

```
solve-inventory-pharmacy/
├── 📁 src/
│   ├── 📦 SolveStation.Common/
│   │   ├── Models/
│   │   ├── Exceptions/
│   │   ├── Extensions/
│   │   └── Interfaces/
│   ├── 📦 SolveStation.Authentication/
│   │   ├── Controllers/
│   │   ├── Services/
│   │   ├── Models/
│   │   └── Validators/
│   ├── 📦 SolveStation.UserManagement/
│   ├── 📦 SolveStation.PrescriptionManagement/
│   ├── 📦 SolveStation.InventoryManagement/
│   ├── 📦 SolveStation.Data/
│   └── 📦 SolveStation.PharmacyApi/
├── 📁 tests/
│   ├── SolveStation.*.Tests/
│   └── SolveStation.PharmacyApi.IntegrationTests/
├── 📁 docs/
│   ├── TEAM_TASK_DELEGATION.md
│   └── architecture/
├── 📁 scripts/
└── 📄 README.md
```

## 🔒 Security

- **JWT Authentication**: 1-hour token expiry with refresh capability
- **Role-Based Access Control**: Granular permission system
- **Input Validation**: Comprehensive request validation
- **HTTPS Only**: All communications encrypted
- **Audit Logging**: Track all user actions

## 📊 Performance Targets

| Metric           | Target | Current       |
|------------------|--------|---------------|
| Response Time    | <2s    | ⏱️ TBD        |
| Concurrent Users | 10+    | 🔄 Testing    |
| Uptime           | 99%+   | 📈 Monitoring |
| Error Rate       | <1%    | 🎯 Tracking   |

## 🚢 Deployment

### Production Deployment

```bash
# Build production image
docker build -t pharmacy-api:latest .

# Run with production compose
docker-compose -f docker-compose.prod.yml up -d

# Health check
curl https://your-domain.com/health
```

### Environment Variables

```bash
DATABASE_CONNECTION_STRING=    # PostgreSQL connection
JWT_SECRET_KEY=               # JWT signing key
JWT_ISSUER=                  # JWT issuer
ASPNETCORE_ENVIRONMENT=      # Environment (Development/Production)
```

## 📞 Support

- **Issues**: [GitHub Issues](https://github.com/SolveStation/solve-inventory-pharmacy/issues)
- **Discussions**: [GitHub Discussions](https://github.com/SolveStation/solve-inventory-pharmacy/discussions)
- **Email**: support@solvestation.com

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- Built with [ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/)
- Database powered by [PostgreSQL](https://www.postgresql.org/)
- Testing with [NUnit](https://nunit.org/)
- Documentation with [Swagger/OpenAPI](https://swagger.io/)

---

**⚡ Ready to revolutionize pharmacy management!**

For detailed setup instructions and contribution guidelines, see
the [Team Task Delegation Guide](docs/TEAM_TASK_DELEGATION.md).
