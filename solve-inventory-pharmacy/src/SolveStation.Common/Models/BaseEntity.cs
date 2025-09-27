namespace SolveStation.Common.Models;

public abstract class BaseEntity<TId> where TId : notnull
{
    public TId Id { get; protected set; } = default!;
    public DateTime CreatedAt { get; protected set; }
    public DateTime UpdatedAt { get; protected set; }
    public bool IsActive { get; protected set; } = true;

    private readonly List<DomainEvent> _domainEvents = new();
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected BaseEntity()
    {
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    protected void AddDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    public void MarkAsUpdated()
    {
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        MarkAsUpdated();
    }

    public void Activate()
    {
        IsActive = true;
        MarkAsUpdated();
    }
}
