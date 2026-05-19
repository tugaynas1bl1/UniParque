using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UniParque.Application.Common;
using UniParque.Application.DTOs;
using UniParque.Application.Services;

namespace UniParque.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ParkingSubSectionsController : ControllerBase
{
    private readonly IParkingSubSectionService _parkingSubSectionService;

    public ParkingSubSectionsController(IParkingSubSectionService parkingSubSectionService)
    {
        _parkingSubSectionService = parkingSubSectionService;
    }

    /// <summary>
    /// Create a new Parking Subsection
    /// </summary>
    /// <param name="createdSubSectionRequestDto">The payload used to add a new subsection</param>
    /// <returns>The created subsection wrapped in ParkingSubSectionResponseDto</returns>
    /// <response code="201">The subsection successfully added</response>
    /// <response code="400">Request body is invalid</response>
    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ApiResponse<ParkingSubSectionResponseDto>>> Create(
        [FromBody] CreateParkingSubSectionRequestDto createdSubSectionRequestDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var createdSubSection = await _parkingSubSectionService.CreateAsync(createdSubSectionRequestDto);

        return CreatedAtAction(nameof(GetById), new { id = createdSubSection.Id },
            ApiResponse<ParkingSubSectionResponseDto>.SuccessResponse(createdSubSection, "Parking subsection created successfully!"));
    }

    /// <summary>
    /// Get a Subsection by ID (Get by ID)
    /// </summary>
    /// <param name="id">The ID of the subsection to retrieve</param>
    /// <returns>The subsection wrapped in ParkingSubSectionResponseDto</returns>
    /// <response code="200">Subsection found</response>
    /// <response code="404">Subsection not found</response>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ParkingSubSectionResponseDto>>> GetById(Guid id)
    {
        var subSection = await _parkingSubSectionService.GetByIdAsync(id);

        if (subSection is null)
            return NotFound("Parking subsection not found!");

        return Ok(ApiResponse<ParkingSubSectionResponseDto>.SuccessResponse(subSection));
    }

    /// <summary>
    /// Get all Subsections (Get All)
    /// </summary>
    /// <returns>All subsections wrapped in ParkingSubSectionResponseDto</returns>
    /// <response code="200">Subsections found</response>
    /// <response code="404">No subsections found</response>
    [HttpGet("all")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ParkingSubSectionResponseDto>>>> GetAll()
    {
        var subSections = await _parkingSubSectionService.GetAllAsync();

        if (subSections is null)
            return NotFound("No subsections found");

        return Ok(ApiResponse<IEnumerable<ParkingSubSectionResponseDto>>.SuccessResponse(subSections));
    }

    /// <summary>
    /// Get all Subsections by Section ID (Get by Branch ID)
    /// </summary>
    /// <param name="sectionId">The ID of the section</param>
    /// <returns>All subsections of the section wrapped in ParkingSubSectionResponseDto</returns>
    /// <response code="200">Subsections found</response>
    /// <response code="404">No subsections found for this section</response>
    [HttpGet("subsections/{sectionId}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ParkingSubSectionResponseDto>>>> GetBySectionId(Guid sectionId)
    {
        var subSections = await _parkingSubSectionService.GetBySectionIdAsync(sectionId);

        if (subSections is null)
            return NotFound("No subsections found for this branch");

        return Ok(ApiResponse<IEnumerable<ParkingSubSectionResponseDto>>.SuccessResponse(subSections));
    }

    /// <summary>
    /// Delete a Parking Subsection by ID (Delete by ID)
    /// </summary>
    /// <param name="id">The ID of the subsection to delete</param>
    /// <returns>True if deleted successfully</returns>
    /// <response code="200">Subsection deleted successfully</response>
    /// <response code="404">Subsection not found or could not be deleted</response>
    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
    {
        var isDeleted = await _parkingSubSectionService.DeleteAsync(id);

        if (!isDeleted)
            return NotFound("The subsection could not be deleted");

        return Ok(ApiResponse<object>.SuccessResponse(isDeleted, "The subsection deleted successfully!"));
    }

    /// <summary>
    /// Delete all Subsections (Delete All)
    /// </summary>
    /// <returns>True if all subsections deleted successfully</returns>
    /// <response code="200">All subsections deleted successfully</response>
    /// <response code="404">No subsections to delete</response>
    [HttpDelete("all")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteAll()
    {
        var isDeleted = await _parkingSubSectionService.DeleteAllAsync();

        if (!isDeleted)
            return NotFound("No subsections to delete");

        return Ok(ApiResponse<object>.SuccessResponse(isDeleted, "All subsections deleted successfully!"));
    }

    /// <summary>
    /// Update a Subsection by ID (Update by ID)
    /// </summary>
    /// <param name="id">The ID of the subsection to update</param>
    /// <param name="updateRequest">The updated subsection data</param>
    /// <returns>The updated subsection wrapped in ParkingSubSectionResponseDto</returns>
    /// <response code="200">Subsection updated successfully</response>
    /// <response code="404">Subsection not found</response>
    [HttpPut]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ApiResponse<ParkingSubSectionResponseDto>>> Update(Guid id, UpdateParkingSubSectionRequestDto updateRequest)
    {
        var updatedSubSection = await _parkingSubSectionService.UpdateAsync(id, updateRequest);

        if (updatedSubSection is null)
            return NotFound("The subsection you want to update was not found!");

        return Ok(ApiResponse<ParkingSubSectionResponseDto>.SuccessResponse(updatedSubSection, "Subsection updated successfully!"));
    }
}
