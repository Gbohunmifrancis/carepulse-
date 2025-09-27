using SolveStation.Common.Models;
using SolveStation.Data.Models;

namespace SolveStation.Data.Models;

// Drug-related domain events
public record DrugCreatedEvent(DrugId DrugId, string Name, int InitialStock) : DomainEvent;
public record StockUpdatedEvent(DrugId DrugId, string DrugName, int PreviousQuantity, int NewQuantity, string Reason, UserId UpdatedBy) : DomainEvent;
public record LowStockAlertEvent(DrugId DrugId, string DrugName, int CurrentStock, int MinimumLevel) : DomainEvent;
public record DrugPriceUpdatedEvent(DrugId DrugId, string DrugName, decimal PreviousPrice, decimal NewPrice, UserId UpdatedBy) : DomainEvent;
public record DrugExpiryAlertEvent(DrugId DrugId, string DrugName, DateTime ExpiryDate) : DomainEvent;

// Prescription-related domain events
public record PrescriptionCreatedEvent(PrescriptionId PrescriptionId, UserId PatientId, UserId DoctorId) : DomainEvent;
public record PrescriptionItemAddedEvent(PrescriptionId PrescriptionId, DrugId DrugId, int Quantity) : DomainEvent;
public record PrescriptionItemRemovedEvent(PrescriptionId PrescriptionId, DrugId DrugId) : DomainEvent;
public record PrescriptionSubmittedEvent(PrescriptionId PrescriptionId, UserId PatientId, UserId DoctorId, decimal TotalAmount) : DomainEvent;
public record PrescriptionFilledEvent(PrescriptionId PrescriptionId, UserId PharmacistId, PrescriptionStatus Status, int ItemsFilled) : DomainEvent;
public record PrescriptionCancelledEvent(PrescriptionId PrescriptionId, string Reason) : DomainEvent;
public record PrescriptionItemFilledEvent(PrescriptionItemId ItemId, PrescriptionId PrescriptionId, DrugId DrugId, int QuantityFilled) : DomainEvent;

// User-related domain events
public record UserCreatedEvent(UserId UserId, Email Email, UserRole Role) : DomainEvent;
public record UserProfileUpdatedEvent(UserId UserId, string FirstName, string LastName) : DomainEvent;
public record UserPasswordChangedEvent(UserId UserId) : DomainEvent;
public record UserEmailVerifiedEvent(UserId UserId, Email Email) : DomainEvent;
