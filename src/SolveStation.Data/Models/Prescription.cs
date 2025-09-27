using SolveStation.Common.Models;

namespace SolveStation.Data.Models;

public record PrescriptionId(Guid Value)
{
    public static PrescriptionId NewId() => new(Guid.NewGuid());
    public static PrescriptionId Empty => new(Guid.Empty);

    public override string ToString() => Value.ToString();
}

public record PrescriptionItemId(Guid Value)
{
    public static PrescriptionItemId NewId() => new(Guid.NewGuid());
    public static PrescriptionItemId Empty => new(Guid.Empty);

    public override string ToString() => Value.ToString();
}

public enum PrescriptionStatus
{
    Created,
    Pending,
    Filled,
    PartiallyFilled,
    Cancelled,
    Expired
}

public class Prescription : BaseEntity<PrescriptionId>
{
    public UserId PatientId { get; private set; } = null!;
    public UserId DoctorId { get; private set; } = null!;
    public PrescriptionStatus Status { get; private set; }
    public string Notes { get; private set; } = string.Empty;
    public DateTime? FilledAt { get; private set; }
    public UserId? FilledByPharmacistId { get; private set; }
    public DateTime? ExpiresAt { get; private set; }
    public decimal TotalAmount { get; private set; }

    private readonly List<PrescriptionItem> _items = new();
    public IReadOnlyCollection<PrescriptionItem> Items => _items.AsReadOnly();

    private Prescription() : base() { }

    public Prescription(PrescriptionId id, UserId patientId, UserId doctorId, string notes = "") : base()
    {
        Id = id;
        PatientId = patientId;
        DoctorId = doctorId;
        Notes = notes;
        Status = PrescriptionStatus.Created;
        ExpiresAt = DateTime.UtcNow.AddDays(30); // Prescriptions expire in 30 days
        TotalAmount = 0;

        AddDomainEvent(new PrescriptionCreatedEvent(id, patientId, doctorId));
    }

    public void AddItem(Drug drug, int quantity, string instructions = "")
    {
        if (Status != PrescriptionStatus.Created)
            throw new InvalidOperationException("Cannot add items to a prescription that is not in Created status");

        var item = new PrescriptionItem(PrescriptionItemId.NewId(), Id, drug.Id, drug.Name,
                                       quantity, drug.Price, instructions);
        _items.Add(item);

        RecalculateTotalAmount();
        MarkAsUpdated();

        AddDomainEvent(new PrescriptionItemAddedEvent(Id, drug.Id, quantity));
    }

    public void RemoveItem(PrescriptionItemId itemId)
    {
        if (Status != PrescriptionStatus.Created)
            throw new InvalidOperationException("Cannot remove items from a prescription that is not in Created status");

        var item = _items.FirstOrDefault(i => i.Id == itemId);
        if (item != null)
        {
            _items.Remove(item);
            RecalculateTotalAmount();
            MarkAsUpdated();

            AddDomainEvent(new PrescriptionItemRemovedEvent(Id, item.DrugId));
        }
    }

    public void Submit()
    {
        if (Status != PrescriptionStatus.Created)
            throw new InvalidOperationException("Only Created prescriptions can be submitted");

        if (!_items.Any())
            throw new InvalidOperationException("Cannot submit empty prescription");

        Status = PrescriptionStatus.Pending;
        MarkAsUpdated();

        AddDomainEvent(new PrescriptionSubmittedEvent(Id, PatientId, DoctorId, TotalAmount));
    }

    public void Fill(UserId pharmacistId, List<PrescriptionItemFillInfo> fillInfo)
    {
        if (Status != PrescriptionStatus.Pending)
            throw new InvalidOperationException("Only Pending prescriptions can be filled");

        if (IsExpired())
            throw new InvalidOperationException("Cannot fill expired prescription");

        var allItemsFilled = true;
        foreach (var info in fillInfo)
        {
            var item = _items.FirstOrDefault(i => i.Id == info.ItemId);
            if (item != null)
            {
                item.Fill(info.QuantityFilled);
                if (!item.IsFullyFilled())
                    allItemsFilled = false;
            }
        }

        Status = allItemsFilled ? PrescriptionStatus.Filled : PrescriptionStatus.PartiallyFilled;
        FilledAt = DateTime.UtcNow;
        FilledByPharmacistId = pharmacistId;
        MarkAsUpdated();
    }

    public void Cancel(string reason)
    {
        if (Status == PrescriptionStatus.Filled || Status == PrescriptionStatus.Cancelled)
            throw new InvalidOperationException("Cannot cancel filled or already cancelled prescription");

        Status = PrescriptionStatus.Cancelled;
        MarkAsUpdated();
    }

    public bool IsExpired() => ExpiresAt.HasValue && ExpiresAt.Value <= DateTime.UtcNow;

    private void RecalculateTotalAmount()
    {
        TotalAmount = _items.Sum(i => i.TotalPrice);
    }
}

public record PrescriptionItemFillInfo(PrescriptionItemId ItemId, int QuantityFilled);
