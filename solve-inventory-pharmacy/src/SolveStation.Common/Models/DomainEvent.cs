namespace SolveStation.Common.Models;

public abstract record DomainEvent(Guid Id, DateTime OccurredAt)
{
    protected DomainEvent() : this(Guid.NewGuid(), DateTime.UtcNow) { }
}

public interface IDomainEventHandler<in TDomainEvent> where TDomainEvent : DomainEvent
{
    Task Handle(TDomainEvent domainEvent, CancellationToken cancellationToken = default);
}
