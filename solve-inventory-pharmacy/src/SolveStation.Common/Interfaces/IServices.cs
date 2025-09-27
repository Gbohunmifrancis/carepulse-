using SolveStation.Common.Models;

namespace SolveStation.Common.Interfaces;

/// <summary>
/// Generic specification pattern interface for complex queries
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public interface ISpecification<T>
{
    bool IsSatisfiedBy(T entity);
    IQueryable<T> Apply(IQueryable<T> query);
}

/// <summary>
/// Interface for domain event handling
/// </summary>
public interface IDomainEventHandler<in T> where T : DomainEvent
{
    Task HandleAsync(T domainEvent, CancellationToken cancellationToken = default);
}

/// <summary>
/// Interface for domain event dispatcher
/// </summary>
public interface IDomainEventDispatcher
{
    Task DispatchAsync(DomainEvent domainEvent, CancellationToken cancellationToken = default);
    Task DispatchAsync(IEnumerable<DomainEvent> domainEvents, CancellationToken cancellationToken = default);
}

/// <summary>
/// Interface for caching operations
/// </summary>
public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default);
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
    Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default);
}

/// <summary>
/// Interface for audit logging
/// </summary>
public interface IAuditLogger
{
    Task LogAsync(string action, string entityType, string entityId, object? oldValue = null, object? newValue = null, CancellationToken cancellationToken = default);
}

/// <summary>
/// Interface for notification services
/// </summary>
public interface INotificationService
{
    Task SendNotificationAsync(string recipientId, string title, string message, CancellationToken cancellationToken = default);
    Task SendEmailAsync(string email, string subject, string body, CancellationToken cancellationToken = default);
}

/// <summary>
/// Interface for background job processing
/// </summary>
public interface IBackgroundJobService
{
    Task<string> EnqueueAsync<T>(T job, CancellationToken cancellationToken = default);
    Task<string> ScheduleAsync<T>(T job, TimeSpan delay, CancellationToken cancellationToken = default);
    Task<string> RecurringAsync<T>(T job, string cronExpression, CancellationToken cancellationToken = default);
}
