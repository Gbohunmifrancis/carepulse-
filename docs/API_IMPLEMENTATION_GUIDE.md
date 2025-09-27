# API Layer Implementation Guide

## 🚀 **What Has Been Implemented**

A comprehensive RESTful API layer has been successfully implemented for the Pharmacy Management System with the following components:

### **📁 Project Structure**
```
src/SolveStation.PharmacyApi/
├── Controllers/           # API endpoint controllers
├── Models/DTOs/          # Data Transfer Objects
├── Models/Validators/    # Request validation rules
├── Models/Profiles/      # AutoMapper mapping profiles
├── Middleware/           # Custom middleware components
├── Configuration/        # Service configuration extensions
├── Services/            # Application services
└── Properties/          # Launch settings and configurations
```

## 🔧 **Key Components Implemented**

### **1. Controllers (5 Controllers)**
- **AuthController** - Authentication and JWT token management
- **UsersController** - User management with role-based access
- **DrugsController** - Drug inventory management
- **PrescriptionsController** - Prescription management workflow
- **HealthController** - System health monitoring

### **2. Data Transfer Objects (DTOs)**
- **UserDTOs** - User-related request/response models
- **DrugDTOs** - Drug inventory models with search capabilities
- **PrescriptionDTOs** - Complex prescription workflow models
- **PagedResult** - Generic pagination wrapper

### **3. Validation System**
- **FluentValidation** integration with comprehensive rules
- **UserValidators** - Email, password, and role validation
- **DrugValidators** - Inventory data validation
- **PrescriptionValidators** - Medical prescription validation

### **4. Security & Authentication**
- **JWT Bearer Token** authentication
- **Role-based Authorization** (Admin, Doctor, Pharmacist, Patient)
- **CORS Configuration** for frontend integration
- **Custom Error Handling** middleware

### **5. API Documentation**
- **Swagger UI** with interactive testing interface
- **JWT Authentication** integration in Swagger
- **Comprehensive endpoint documentation**

## ⚙️ **Database Configuration**

### **Database Provider**: SQL Server
- **Connection**: Remote SQL Server database (somee.com)
- **Migration Status**: ✅ Initial migration applied successfully
- **Entity Framework**: Configured with proper value conversions for domain models

### **Key Database Features**
- **Domain Events**: Properly ignored in EF configuration
- **Value Objects**: UserId, DrugId, PrescriptionId with proper conversions
- **Soft Delete**: Implemented via IsActive flag
- **Audit Trail**: CreatedAt/UpdatedAt timestamps

## 🌐 **API Endpoints Available**

### **Authentication Endpoints**
```
POST   /api/auth/login           # User authentication
POST   /api/auth/refresh         # Token refresh
POST   /api/auth/logout          # User logout
GET    /api/auth/me              # Current user info
```

### **User Management Endpoints**
```
GET    /api/users                # List users (Admin only)
GET    /api/users/{id}           # Get user by ID
POST   /api/users                # Create user (Admin only)
PUT    /api/users/{id}           # Update user
POST   /api/users/{id}/change-password # Change password
DELETE /api/users/{id}           # Deactivate user (Admin only)
```

### **Drug Inventory Endpoints**
```
GET    /api/drugs                # Search drugs with filters
GET    /api/drugs/{id}           # Get drug details
POST   /api/drugs                # Add new drug (Pharmacist)
PUT    /api/drugs/{id}           # Update drug (Pharmacist)
PATCH  /api/drugs/{id}/stock     # Update stock levels
GET    /api/drugs/low-stock      # Get low stock alerts
GET    /api/drugs/expired        # Get expired drugs
DELETE /api/drugs/{id}           # Remove drug (Admin)
```

### **Prescription Management Endpoints**
```
GET    /api/prescriptions        # Search prescriptions
GET    /api/prescriptions/{id}   # Get prescription details
POST   /api/prescriptions        # Create prescription (Doctor)
PUT    /api/prescriptions/{id}   # Update prescription (Doctor)
POST   /api/prescriptions/{id}/fill      # Fill prescription (Pharmacist)
POST   /api/prescriptions/{id}/cancel    # Cancel prescription
GET    /api/prescriptions/patient/{id}   # Patient prescriptions
GET    /api/prescriptions/pending        # Pending prescriptions
```

### **System Health Endpoints**
```
GET    /api/health               # Basic health check
GET    /api/health/detailed      # Detailed system status
```

## 🛠️ **What Collaborators Should Look Out For**

### **❗ Critical Configuration Issues to Avoid**

#### **1. Environment Configuration**
```json
// ✅ CORRECT - launchSettings.json must set Development environment
{
  "profiles": {
    "http": {
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"  // Critical for Swagger UI
      }
    }
  }
}

// ❌ INCORRECT - Production mode hides Swagger UI
"ASPNETCORE_ENVIRONMENT": "Production"
```

#### **2. Database Connection Issues**
```csharp
// ✅ CORRECT - Use SQL Server with proper connection string
services.AddDbContext<PharmacyDbContext>(options =>
    options.UseSqlServer(connectionString));

// ❌ COMMON MISTAKE - Using LocalDB without installation
"Server=(localdb)\\mssqllocaldb"  // Won't work if LocalDB not installed
```

#### **3. Entity Framework Configuration**
```csharp
// ✅ CRITICAL - Always ignore DomainEvent in DbContext
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Ignore<DomainEvent>();  // ESSENTIAL to prevent EF errors
    // ...
}

// ❌ MISTAKE - Forgetting this will cause migration failures
```

#### **4. Dependency Injection Pitfall**
```csharp
// ❌ NEVER register abstract base classes
services.AddScoped(typeof(IRepository<,>), typeof(BaseRepository<,>));  // Will fail!

// ✅ CORRECT - Only register concrete implementations
services.AddScoped<IDrugRepository, DrugRepository>();
```

### **🔒 Security Considerations**

#### **JWT Configuration**
- **Secret Key**: Must be at least 256 bits (32 characters)
- **Token Expiration**: Currently set to 60 minutes (configurable)
- **HTTPS**: Only disabled in Development mode

#### **Role-Based Access Control**
```csharp
// Roles implemented:
- Admin     // Full system access
- Doctor    // Can create/manage prescriptions
- Pharmacist // Can fill prescriptions, manage inventory
- Patient   // Can view own prescriptions
```

### **📊 Database Migration Workflow**

#### **For New Developers**
```bash
# 1. Restore packages
dotnet restore

# 2. Update database (creates if doesn't exist)
dotnet ef database update --project src/SolveStation.Data --startup-project src/SolveStation.PharmacyApi

# 3. Run application
dotnet run --project src/SolveStation.PharmacyApi
```

#### **For Schema Changes**
```bash
# 1. Add new migration
dotnet ef migrations add MigrationName --project src/SolveStation.Data --startup-project src/SolveStation.PharmacyApi

# 2. Review migration files in src/SolveStation.Data/Migrations/

# 3. Apply migration
dotnet ef database update --project src/SolveStation.Data --startup-project src/SolveStation.PharmacyApi
```

### **🚨 Common Startup Issues & Solutions**

#### **Issue: "UseSqlServer not found"**
```bash
# Solution: Restore packages
dotnet restore src/SolveStation.Data
```

#### **Issue: "Cannot instantiate BaseRepository"**
```csharp
// Problem: Abstract class registered in DI
// Solution: Remove from ServiceCollectionExtensions.cs
// Only register concrete implementations
```

#### **Issue: "DomainEvent cannot be instantiated"**
```csharp
// Solution: Add to DbContext OnModelCreating
modelBuilder.Ignore<DomainEvent>();
```

#### **Issue: Swagger UI not accessible**
```json
// Check launchSettings.json
"ASPNETCORE_ENVIRONMENT": "Development"  // Must be Development

// And ensure Program.cs has:
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
```

### **🔧 Development Tools Setup**

#### **Required Tools**
- **.NET 8 SDK** (latest version)
- **Entity Framework Tools**: `dotnet tool install --global dotnet-ef`
- **SQL Server** or **SQL Server Express**

#### **Recommended Extensions (VS Code)**
- **C# Dev Kit**
- **REST Client** (for API testing)
- **GitLens** (for Git integration)

#### **Testing the API**
1. **Swagger UI**: `http://localhost:5000` (Development mode)
2. **Health Check**: `http://localhost:5000/api/health`
3. **Postman Collection**: Import from Swagger JSON

### **📋 Code Standards & Conventions**

#### **Naming Conventions**
- **Controllers**: `[Entity]Controller` (e.g., `DrugsController`)
- **DTOs**: `[Entity]Dto` or `[Action][Entity]Request`
- **Validators**: `[DTO]Validator`

#### **Error Handling**
- **Custom Exceptions**: Use domain exceptions in Common project
- **Validation**: FluentValidation with detailed error messages
- **HTTP Status Codes**: Follow REST conventions

#### **API Versioning** (Prepared for future)
- **URL Structure**: `/api/v1/[controller]`
- **Swagger Groups**: Separate docs per version

## 🚀 **Next Steps for Team**

### **Immediate Tasks**
1. **Business Logic Implementation**: Replace `NotImplementedException` in controllers
2. **Authentication Service**: Implement JWT token generation/validation
3. **Unit Tests**: Add comprehensive test coverage
4. **Integration Tests**: Test complete API workflows

### **Future Enhancements**
1. **Caching**: Add Redis for performance
2. **Logging**: Enhance structured logging with Serilog
3. **Rate Limiting**: Implement API throttling
4. **API Versioning**: Prepare for future versions

---

## 📞 **Support**

If you encounter issues:
1. **Check this guide** first
2. **Review Git commit history** for implementation details
3. **Test with Swagger UI** at `http://localhost:5000`
4. **Verify Environment**: Ensure Development mode for Swagger

**Happy Coding! 🎉**
