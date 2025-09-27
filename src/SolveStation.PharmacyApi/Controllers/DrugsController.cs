using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SolveStation.PharmacyApi.Models.DTOs;
using SolveStation.PharmacyApi.Services;

namespace SolveStation.PharmacyApi.Controllers;

/// <summary>
/// Drug inventory management controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class DrugsController : ControllerBase
{
    private readonly ILogger<DrugsController> _logger;
    private readonly IDrugService _drugService;

    public DrugsController(ILogger<DrugsController> logger, IDrugService drugService)
    {
        _logger = logger;
        _drugService = drugService;
    }

    /// <summary>
    /// Search and get drugs with pagination and filtering
    /// </summary>
    /// <param name="request">Search criteria</param>
    /// <returns>Paginated list of drugs</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<DrugDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDrugs([FromQuery] DrugSearchRequest request)
    {
        _logger.LogInformation("Searching drugs with criteria: {Request}", request);

        try
        {
            var result = await _drugService.SearchDrugsAsync(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching drugs");
            return BadRequest(new { message = "An error occurred while searching drugs" });
        }
    }

    /// <summary>
    /// Get drug by ID
    /// </summary>
    /// <param name="id">Drug ID</param>
    /// <returns>Drug details</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(DrugDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDrug(Guid id)
    {
        _logger.LogInformation("Getting drug with ID: {DrugId}", id);

        try
        {
            var drug = await _drugService.GetDrugByIdAsync(id);
            return Ok(drug);
        }
        catch (SolveStation.Common.Exceptions.NotFoundException ex)
        {
            _logger.LogWarning("Drug not found: {Message}", ex.Message);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting drug {DrugId}", id);
            return BadRequest(new { message = "An error occurred while getting drug" });
        }
    }

    /// <summary>
    /// Create a new drug
    /// </summary>
    /// <param name="request">Drug creation data</param>
    /// <returns>Created drug</returns>
    [HttpPost]
    [Authorize(Policy = "RequirePharmacistRole")]
    [ProducesResponseType(typeof(DrugDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateDrug([FromBody] CreateDrugRequest request)
    {
        _logger.LogInformation("Creating new drug: {DrugName}", request.Name);

        try
        {
            var drug = await _drugService.CreateDrugAsync(request);
            return CreatedAtAction(nameof(GetDrug), new { id = drug.Id }, drug);
        }
        catch (SolveStation.Common.Exceptions.ValidationException ex)
        {
            _logger.LogWarning("Validation error creating drug: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating drug {DrugName}", request.Name);
            return BadRequest(new { message = "An error occurred while creating drug" });
        }
    }

    /// <summary>
    /// Update drug information
    /// </summary>
    /// <param name="id">Drug ID</param>
    /// <param name="request">Updated drug data</param>
    /// <returns>Updated drug</returns>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "RequirePharmacistRole")]
    [ProducesResponseType(typeof(DrugDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateDrug(Guid id, [FromBody] UpdateDrugRequest request)
    {
        _logger.LogInformation("Updating drug with ID: {DrugId}", id);

        try
        {
            var drug = await _drugService.UpdateDrugAsync(id, request);
            return Ok(drug);
        }
        catch (SolveStation.Common.Exceptions.NotFoundException ex)
        {
            _logger.LogWarning("Drug not found: {Message}", ex.Message);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating drug {DrugId}", id);
            return BadRequest(new { message = "An error occurred while updating drug" });
        }
    }

    /// <summary>
    /// Update drug stock quantity
    /// </summary>
    /// <param name="id">Drug ID</param>
    /// <param name="request">Stock update data</param>
    /// <returns>Updated drug</returns>
    [HttpPatch("{id:guid}/stock")]
    [Authorize(Policy = "RequirePharmacistRole")]
    [ProducesResponseType(typeof(DrugDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateStock(Guid id, [FromBody] UpdateStockRequest request)
    {
        _logger.LogInformation("Updating stock for drug ID: {DrugId} to {NewQuantity}", id, request.NewQuantity);

        try
        {
            var drug = await _drugService.UpdateStockAsync(id, request);
            return Ok(drug);
        }
        catch (SolveStation.Common.Exceptions.NotFoundException ex)
        {
            _logger.LogWarning("Drug not found: {Message}", ex.Message);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating stock for drug {DrugId}", id);
            return BadRequest(new { message = "An error occurred while updating stock" });
        }
    }

    /// <summary>
    /// Get low stock drugs
    /// </summary>
    /// <returns>List of drugs with low stock</returns>
    [HttpGet("low-stock")]
    [Authorize(Policy = "RequirePharmacistRole")]
    [ProducesResponseType(typeof(List<DrugDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLowStockDrugs()
    {
        _logger.LogInformation("Getting low stock drugs");

        try
        {
            var drugs = await _drugService.GetLowStockDrugsAsync();
            return Ok(drugs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting low stock drugs");
            return BadRequest(new { message = "An error occurred while getting low stock drugs" });
        }
    }

    /// <summary>
    /// Get expired drugs
    /// </summary>
    /// <returns>List of expired drugs</returns>
    [HttpGet("expired")]
    [Authorize(Policy = "RequirePharmacistRole")]
    [ProducesResponseType(typeof(List<DrugDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetExpiredDrugs()
    {
        _logger.LogInformation("Getting expired drugs");

        try
        {
            var drugs = await _drugService.GetExpiredDrugsAsync();
            return Ok(drugs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting expired drugs");
            return BadRequest(new { message = "An error occurred while getting expired drugs" });
        }
    }

    /// <summary>
    /// Delete drug (soft delete)
    /// </summary>
    /// <param name="id">Drug ID</param>
    /// <returns>Success confirmation</returns>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "RequireAdminRole")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteDrug(Guid id)
    {
        _logger.LogInformation("Deleting drug with ID: {DrugId}", id);

        try
        {
            var success = await _drugService.DeleteDrugAsync(id);
            if (!success)
                return NotFound(new { message = $"Drug with ID {id} not found" });

            return Ok(new { message = "Drug deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting drug {DrugId}", id);
            return BadRequest(new { message = "An error occurred while deleting drug" });
        }
    }
}
