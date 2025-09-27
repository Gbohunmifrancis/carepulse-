# SolveStation.Data Layer

## Overview

The Data layer implements the Repository pattern with Unit of Work, following Domain-Driven Design principles and Clean
Architecture patterns.

## Key Components

### Models

- **Drug**: Represents medications with inventory tracking, expiry dates, and low stock alerts
- **Prescription**: Manages prescriptions with items, status tracking, and workflow
- **PrescriptionItem**: Individual items within a prescription
- **User**: User entity with role-based access and domain events

### Repositories

- **IDrugRepository**: Drug-specific queries including low stock, expired drugs, and search
- **IPrescriptionRepository**: Prescription queries by patient, doctor, status, etc.
- **IUserRepository**: User queries by role, email, and activity status
- **BaseRepository**: Common CRUD operations for all entities

### Unit of Work

- **IUnitOfWork**: Coordinates multiple repositories and manages transactions
- Provides atomic operations across multiple aggregates

### Database Context

- **PharmacyDbContext**: Entity Framework Core context with proper configurations
- Includes value object conversions and domain event handling

## Features

- Value Object support (DrugId, PrescriptionId, UserId, Email)
- Domain Events for business rule enforcement
- Proper entity configurations with indexes
- Transaction support via Unit of Work
- Seed data for initial setup
- Optimistic concurrency handling

## Usage

### Dependency Injection

```csharp
services.AddDataLayer(connectionString);
```

### Repository Usage

```csharp
public class DrugService
{
    private readonly IUnitOfWork _unitOfWork;
    
    public async Task<Drug> CreateDrugAsync(AddDrugRequest request)
    {
        var drug = new Drug(/*...*/);
        await _unitOfWork.Drugs.AddAsync(drug);
        await _unitOfWork.SaveChangesAsync();
        return drug;
    }
}
```

### Transaction Usage

```csharp
await _unitOfWork.BeginTransactionAsync();
try
{
    // Multiple operations
    await _unitOfWork.SaveChangesAsync();
    await _unitOfWork.CommitTransactionAsync();
}
catch
{
    await _unitOfWork.RollbackTransactionAsync();
    throw;
}
```
