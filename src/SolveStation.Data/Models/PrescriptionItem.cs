using SolveStation.Common.Models;

namespace SolveStation.Data.Models;

public class PrescriptionItem : BaseEntity<PrescriptionItemId>
{
    public PrescriptionId PrescriptionId { get; private set; }
    public DrugId DrugId { get; private set; }
    public string DrugName { get; private set; }
    public int QuantityPrescribed { get; private set; }
    public int QuantityFilled { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal TotalPrice { get; private set; }
    public string Instructions { get; private set; }

    // Navigation properties
    public Prescription Prescription { get; private set; } = null!;
    public Drug Drug { get; private set; } = null!;

    private PrescriptionItem() : base() { }

    public PrescriptionItem(PrescriptionItemId id, PrescriptionId prescriptionId, DrugId drugId,
                           string drugName, int quantity, decimal unitPrice, string instructions = "") : base()
    {
        Id = id;
        PrescriptionId = prescriptionId;
        DrugId = drugId;
        DrugName = drugName;
        QuantityPrescribed = quantity;
        QuantityFilled = 0;
        UnitPrice = unitPrice;
        TotalPrice = quantity * unitPrice;
        Instructions = instructions;
    }

    public void Fill(int quantityFilled)
    {
        if (quantityFilled > QuantityPrescribed - QuantityFilled)
            throw new InvalidOperationException("Cannot fill more than prescribed quantity");

        QuantityFilled += quantityFilled;
        MarkAsUpdated();

        AddDomainEvent(new PrescriptionItemFilledEvent(Id, PrescriptionId, DrugId, quantityFilled));
    }

    public bool IsFullyFilled() => QuantityFilled >= QuantityPrescribed;
    public int RemainingQuantity() => QuantityPrescribed - QuantityFilled;
    public decimal FilledValue() => QuantityFilled * UnitPrice;
}
