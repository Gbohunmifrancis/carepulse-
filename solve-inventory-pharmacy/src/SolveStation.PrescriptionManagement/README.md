# Prescription Management Module

## Overview
The Prescription Management module provides comprehensive functionality for managing medical prescriptions in the pharmacy system. It handles the complete prescription lifecycle from creation to fulfillment.

## Features

### Core Functionality
- **Prescription Creation**: Doctors can create prescriptions with multiple drug items
- **Prescription Management**: Update, submit, and track prescription status
- **Prescription Filling**: Pharmacists can fill prescriptions (full or partial)
- **Prescription Search**: Advanced search and filtering capabilities
- **Status Tracking**: Real-time status updates throughout the prescription lifecycle

### Prescription Statuses
- `Created` - Initial state when prescription is created
- `Pending` - Submitted and waiting for pharmacist to fill
- `Filled` - Completely filled by pharmacist
- `PartiallyFilled` - Some items filled, others pending
- `Cancelled` - Prescription cancelled
- `Expired` - Prescription past expiration date (30 days from creation)

## API Endpoints

### Core Operations
```http
POST   /api/prescriptions                    # Create new prescription
GET    /api/prescriptions/{id}               # Get prescription by ID
PUT    /api/prescriptions/{id}               # Update prescription (Created status only)
DELETE /api/prescriptions/{id}               # Delete prescription (soft delete)
```

### Workflow Operations
```http
POST   /api/prescriptions/{id}/submit        # Submit prescription for filling
POST   /api/prescriptions/{id}/fill          # Fill prescription (pharmacist)
POST   /api/prescriptions/{id}/cancel        # Cancel prescription
```

### Query Operations
```http
GET    /api/prescriptions/search             # Search prescriptions with filters
GET    /api/prescriptions/patient/{id}       # Get prescriptions by patient
GET    /api/prescriptions/doctor/{id}        # Get prescriptions by doctor
GET    /api/prescriptions/pending            # Get pending prescriptions
GET    /api/prescriptions/expired            # Get expired prescriptions
```

## Request/Response Models

### Create Prescription
```json
{
  "patientId": "guid",
  "doctorId": "guid", 
  "notes": "string",
  "items": [
    {
      "drugId": "guid",
      "quantity": 30,
      "instructions": "Take 1 tablet daily"
    }
  ]
}
```

### Fill Prescription
```json
{
  "pharmacistId": "guid",
  "items": [
    {
      "itemId": "guid",
      "quantityFilled": 30
    }
  ]
}
```

### Prescription Response
```json
{
  "id": "guid",
  "patientId": "guid",
  "doctorId": "guid",
  "status": "Pending",
  "notes": "string",
  "filledAt": "2025-09-10T12:00:00Z",
  "filledByPharmacistId": "guid",
  "expiresAt": "2025-10-10T12:00:00Z",
  "totalAmount": 150.00,
  "items": [
    {
      "id": "guid",
      "drugId": "guid",
      "drugName": "Aspirin 100mg",
      "quantityPrescribed": 30,
      "quantityFilled": 30,
      "unitPrice": 5.00,
      "totalPrice": 150.00,
      "instructions": "Take 1 tablet daily",
      "isFullyFilled": true,
      "remainingQuantity": 0
    }
  ],
  "createdAt": "2025-09-10T12:00:00Z",
  "updatedAt": "2025-09-10T12:00:00Z"
}
```

## Business Rules

### Creation Rules
- Only doctors can create prescriptions
- At least one prescription item is required
- Maximum 20 items per prescription
- Prescriptions expire 30 days after creation

### Update Rules
- Only prescriptions in `Created` status can be updated
- Cannot modify prescriptions once submitted

### Filling Rules
- Only `Pending` prescriptions can be filled
- Cannot fill expired prescriptions
- Partial fills are allowed
- Quantity filled cannot exceed quantity prescribed

### Cancellation Rules
- Cannot cancel `Filled` prescriptions
- Can cancel prescriptions in any other status

## Validation

### Input Validation
- All GUIDs validated for format
- Quantity limits enforced (1-1000)
- Text field length limits
- Required field validation

### Business Validation
- Patient and doctor existence verified
- Drug availability checked
- Prescription status rules enforced
- Expiration dates validated

## Domain Events

The module publishes the following domain events:
- `PrescriptionCreatedEvent`
- `PrescriptionItemAddedEvent`
- `PrescriptionItemRemovedEvent`
- `PrescriptionSubmittedEvent`
- `PrescriptionFilledEvent`
- `PrescriptionCancelledEvent`
- `PrescriptionItemFilledEvent`

## Dependencies

### Required Projects
- `SolveStation.Common` - Base entities and exceptions
- `SolveStation.Data` - Data models and repositories

### Required Packages
- `FluentValidation` (11.8.0) - Input validation
- `Microsoft.Extensions.Logging` - Logging
- `Microsoft.AspNetCore.Mvc` - API controllers

## Configuration

Register the prescription management services in your DI container:

```csharp
services.AddPrescriptionManagement();
```

## Security Considerations

### Authorization Policies
- **RequireDoctorRole**: For creating/updating prescriptions
- **RequirePharmacistRole**: For filling prescriptions
- **RequireAdminRole**: For administrative operations

### Data Protection
- Sensitive medical information is handled securely
- Audit trails maintained for all operations
- Soft deletes preserve data integrity

## Usage Examples

### Creating a Prescription (Doctor)
```csharp
var request = new CreatePrescriptionRequest(
    PatientId: "patient-guid",
    DoctorId: "doctor-guid", 
    Notes: "For high blood pressure",
    Items: new List<CreatePrescriptionItemRequest>
    {
        new("drug-guid", 30, "Take 1 tablet daily with food")
    }
);

var prescription = await prescriptionService.CreatePrescriptionAsync(request);
```

### Filling a Prescription (Pharmacist)
```csharp
var fillRequest = new FillPrescriptionRequest(
    PharmacistId: "pharmacist-guid",
    Items: new List<FillPrescriptionItemRequest>
    {
        new("item-guid", 30)
    }
);

var filledPrescription = await prescriptionService.FillPrescriptionAsync(
    prescriptionId, fillRequest);
```

### Searching Prescriptions
```csharp
var searchRequest = new PrescriptionSearchRequest(
    PatientId: "patient-guid",
    Status: PrescriptionStatus.Pending,
    StartDate: DateTime.Today.AddDays(-30),
    Page: 1,
    PageSize: 20
);

var results = await prescriptionService.SearchPrescriptionsAsync(searchRequest);
```

## Testing

The module includes comprehensive unit tests covering:
- Service layer business logic
- Validation rules
- Domain model behavior
- Repository operations
- API controllers

## Performance Considerations

- Database queries optimized with proper indexing
- Pagination implemented for large result sets
- Eager loading for related entities
- Async/await pattern throughout

## Monitoring and Logging

All operations are logged with appropriate detail levels:
- Information: Successful operations
- Warning: Business rule violations
- Error: System errors and exceptions

## Future Enhancements

- Electronic prescribing integration
- Drug interaction checking
- Insurance verification
- Automated refill reminders
- Mobile app support
