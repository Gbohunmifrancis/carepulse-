using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SolveStation.PrescriptionManagement.Models;
using SolveStation.PrescriptionManagement.Services;
using SolveStation.Data.Models;

namespace SolveStation.PrescriptionManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PrescriptionsController : ControllerBase
{
    private readonly IPrescriptionService _prescriptionService;
    private readonly ILogger<PrescriptionsController> _logger;

    public PrescriptionsController(
        IPrescriptionService prescriptionService,
        ILogger<PrescriptionsController> logger)
    {
        _prescriptionService = prescriptionService;
        _logger = logger;
    }


    /// Create a new prescription
    [HttpPost]
    [Authorize(Policy = "RequireDoctorRole")]
    public async Task<ActionResult<PrescriptionResponse>> CreatePrescription(
        [FromBody] CreatePrescriptionRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var prescription = await _prescriptionService.CreatePrescriptionAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetPrescription), new { id = prescription.Id }, prescription);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating prescription");
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Get prescription by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<PrescriptionResponse>> GetPrescription(
        string id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var prescription = await _prescriptionService.GetPrescriptionByIdAsync(id, cancellationToken);
            return Ok(prescription);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting prescription {PrescriptionId}", id);
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Update prescription (only when in Created status)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Policy = "RequireDoctorRole")]
    public async Task<ActionResult<PrescriptionResponse>> UpdatePrescription(
        string id,
        [FromBody] UpdatePrescriptionRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var prescription = await _prescriptionService.UpdatePrescriptionAsync(id, request, cancellationToken);
            return Ok(prescription);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating prescription {PrescriptionId}", id);
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Submit prescription for filling
    /// </summary>
    [HttpPost("{id}/submit")]
    public async Task<ActionResult<PrescriptionResponse>> SubmitPrescription(
        string id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var prescription = await _prescriptionService.SubmitPrescriptionAsync(id, cancellationToken);
            return Ok(prescription);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting prescription {PrescriptionId}", id);
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Fill prescription (pharmacist action)
    /// </summary>
    [HttpPost("{id}/fill")]
    [Authorize(Policy = "RequirePharmacistRole")]
    public async Task<ActionResult<PrescriptionResponse>> FillPrescription(
        string id,
        [FromBody] FillPrescriptionRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var prescription = await _prescriptionService.FillPrescriptionAsync(id, request, cancellationToken);
            return Ok(prescription);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error filling prescription {PrescriptionId}", id);
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Cancel prescription
    /// </summary>
    [HttpPost("{id}/cancel")]
    public async Task<ActionResult<PrescriptionResponse>> CancelPrescription(
        string id,
        [FromBody] CancelPrescriptionRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var prescription = await _prescriptionService.CancelPrescriptionAsync(id, request.Reason, cancellationToken);
            return Ok(prescription);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling prescription {PrescriptionId}", id);
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Search prescriptions with filters
    /// </summary>
    [HttpGet("search")]
    public async Task<ActionResult<PrescriptionSearchResponse>> SearchPrescriptions(
        [FromQuery] PrescriptionSearchRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _prescriptionService.SearchPrescriptionsAsync(request, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching prescriptions");
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Get prescriptions by patient
    /// </summary>
    [HttpGet("patient/{patientId}")]
    public async Task<ActionResult<List<PrescriptionSummaryResponse>>> GetPrescriptionsByPatient(
        string patientId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var prescriptions = await _prescriptionService.GetPrescriptionsByPatientAsync(patientId, cancellationToken);
            return Ok(prescriptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting prescriptions for patient {PatientId}", patientId);
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Get prescriptions by doctor
    /// </summary>
    [HttpGet("doctor/{doctorId}")]
    public async Task<ActionResult<List<PrescriptionSummaryResponse>>> GetPrescriptionsByDoctor(
        string doctorId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var prescriptions = await _prescriptionService.GetPrescriptionsByDoctorAsync(doctorId, cancellationToken);
            return Ok(prescriptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting prescriptions for doctor {DoctorId}", doctorId);
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Get pending prescriptions (for pharmacists)
    /// </summary>
    [HttpGet("pending")]
    [Authorize(Policy = "RequirePharmacistRole")]
    public async Task<ActionResult<List<PrescriptionSummaryResponse>>> GetPendingPrescriptions(
        CancellationToken cancellationToken = default)
    {
        try
        {
            var prescriptions = await _prescriptionService.GetPendingPrescriptionsAsync(cancellationToken);
            return Ok(prescriptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pending prescriptions");
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Get expired prescriptions
    /// </summary>
    [HttpGet("expired")]
    public async Task<ActionResult<List<PrescriptionSummaryResponse>>> GetExpiredPrescriptions(
        CancellationToken cancellationToken = default)
    {
        try
        {
            var prescriptions = await _prescriptionService.GetExpiredPrescriptionsAsync(cancellationToken);
            return Ok(prescriptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting expired prescriptions");
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Delete prescription (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeletePrescription(
        string id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var deleted = await _prescriptionService.DeletePrescriptionAsync(id, cancellationToken);
            if (!deleted)
                return NotFound($"Prescription with ID {id} not found");

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting prescription {PrescriptionId}", id);
            return BadRequest(ex.Message);
        }
    }
}

public record CancelPrescriptionRequest(string Reason);
