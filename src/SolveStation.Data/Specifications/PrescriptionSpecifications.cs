using SolveStation.Data.Models;

namespace SolveStation.Data.Specifications;

public class PrescriptionSpecifications
{
    public class PrescriptionsByPatient : BaseSpecification<Prescription>
    {
        public PrescriptionsByPatient(UserId patientId)
        {
            SetCriteria(p => p.IsActive && p.PatientId.Equals(patientId));
            ApplyOrderByDescending(p => p.CreatedAt);
        }
    }

    public class PrescriptionsByDoctor : BaseSpecification<Prescription>
    {
        public PrescriptionsByDoctor(UserId doctorId)
        {
            SetCriteria(p => p.IsActive && p.DoctorId.Equals(doctorId));
            ApplyOrderByDescending(p => p.CreatedAt);
        }
    }

    public class PrescriptionsByStatus : BaseSpecification<Prescription>
    {
        public PrescriptionsByStatus(PrescriptionStatus status)
        {
            SetCriteria(p => p.IsActive && p.Status == status);
            ApplyOrderByDescending(p => p.CreatedAt);
        }
    }

    public class PendingPrescriptions : BaseSpecification<Prescription>
    {
        public PendingPrescriptions()
        {
            SetCriteria(p => p.IsActive && p.Status == PrescriptionStatus.Pending);
            ApplyOrderBy(p => p.CreatedAt);
        }
    }

    public class ExpiredPrescriptions : BaseSpecification<Prescription>
    {
        public ExpiredPrescriptions()
        {
            var currentDate = DateTime.UtcNow;
            SetCriteria(p => p.IsActive &&
                p.ExpiresAt.HasValue &&
                p.ExpiresAt.Value <= currentDate &&
                p.Status != PrescriptionStatus.Filled &&
                p.Status != PrescriptionStatus.Cancelled);
            ApplyOrderBy(p => p.ExpiresAt);
        }
    }

    public class PrescriptionWithItems : BaseSpecification<Prescription>
    {
        public PrescriptionWithItems(PrescriptionId prescriptionId)
        {
            SetCriteria(p => p.IsActive && p.Id.Equals(prescriptionId));
            AddInclude(p => p.Items);
            AddInclude("Items.Drug");
        }
    }

    public class PrescriptionsByDateRange : BaseSpecification<Prescription>
    {
        public PrescriptionsByDateRange(DateTime startDate, DateTime endDate)
        {
            SetCriteria(p => p.IsActive && p.CreatedAt >= startDate && p.CreatedAt <= endDate);
            ApplyOrderByDescending(p => p.CreatedAt);
        }
    }

    public class FilledPrescriptionsByPharmacist : BaseSpecification<Prescription>
    {
        public FilledPrescriptionsByPharmacist(UserId pharmacistId)
        {
            SetCriteria(p => p.IsActive &&
                p.FilledByPharmacistId != null &&
                p.FilledByPharmacistId.Equals(pharmacistId));
            ApplyOrderByDescending(p => p.FilledAt);
        }
    }

    public class HighValuePrescriptions : BaseSpecification<Prescription>
    {
        public HighValuePrescriptions(decimal minimumAmount)
        {
            SetCriteria(p => p.IsActive && p.TotalAmount >= minimumAmount);
            ApplyOrderByDescending(p => p.TotalAmount);
        }
    }
}
