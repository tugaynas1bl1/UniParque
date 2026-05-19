using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UniParque.Application.Common;
using UniParque.Application.DTOs;
using UniParque.Application.Services;

namespace UniParque.API.Controllers;

[Route("api/[controller]")]
[ApiController]

public class ParkingSectionsController : ControllerBase
{
    private readonly IParkingSectionService _parkingSectionService;

    public ParkingSectionsController(IParkingSectionService parkingSectionService)
    {
        _parkingSectionService = parkingSectionService;
    }

    /// <summary>
    /// Create a new Parking Section
    /// </summary>
    /// <param name="createdSectionRequestDto">The payload used to add a new section</param>
    /// <returns>The created section wrapped in ParkingSectionResponseDto</returns>
    /// <response code="201">The section successfully added</response>
    /// <response code="400">Request body is invalid</response>
    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ApiResponse<ParkingSectionResponseDto>>> Create(
        [FromBody] CreateParkingSectionRequestDto createdSectionRequestDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var createdSection = await _parkingSectionService.CreateAsync(createdSectionRequestDto);

        return CreatedAtAction(nameof(GetById), new { id = createdSection.Id },
            ApiResponse<ParkingSectionResponseDto>.SuccessResponse(createdSection, "Parking section created successfully!"));
    }

    /// <summary>
    /// Get a Section by ID (Get by ID)
    /// </summary>
    /// <param name="id">The ID of the section to retrieve</param>
    /// <returns>The section wrapped in ParkingSectionResponseDto</returns>
    /// <response code="200">Section found</response>
    /// <response code="404">Section not found</response>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ParkingSectionResponseDto>>> GetById(Guid id)
    {
        var section = await _parkingSectionService.GetByIdAsync(id);

        if (section is null)
            return NotFound("Parking section not found!");

        return Ok(ApiResponse<ParkingSectionResponseDto>.SuccessResponse(section));
    }

    /// <summary>
    /// Get all Sections
    /// </summary>
    /// <returns>All sections wrapped in ParkingSectionResponseDto</returns>
    /// <response code="200">Sections found</response>
    /// <response code="404">No sections found</response>
    [HttpGet("all")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ParkingSectionResponseDto>>>> GetAll()
    {
        var sections = await _parkingSectionService.GetAllAsync();

        if (sections is null)
            return NotFound("No sections found");

        return Ok(ApiResponse<IEnumerable<ParkingSectionResponseDto>>.SuccessResponse(sections));
    }

    /// <summary>
    /// Get sections by Branch ID (Get by Branch ID)
    /// </summary>
    /// <param name="branchId">The ID of the branch</param>
    /// <returns>All sections of the branch wrapped in ParkingSectionResponseDto</returns>
    /// <response code="200">Sections found</response>
    /// <response code="404">No sections found for this branch</response>
    [HttpGet("sections/{branchId}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ParkingSectionResponseDto>>>> GetByBranchId(Guid branchId)
    {
        var sections = await _parkingSectionService.GetByBranchIdAsync(branchId);

        if (sections is null)
            return NotFound("No sections found for this branch");

        return Ok(ApiResponse<IEnumerable<ParkingSectionResponseDto>>.SuccessResponse(sections));
    }

    /// <summary>
    /// Delete a Section by ID (Delete by ID)
    /// </summary>
    /// <param name="id">The ID of the section to delete</param>
    /// <returns>True if deleted successfully</returns>
    /// <response code="200">Section deleted successfully</response>
    /// <response code="404">Section not found or could not be deleted</response>
    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
    {
        var isDeleted = await _parkingSectionService.DeleteAsync(id);

        if (!isDeleted)
            return NotFound("The section could not be deleted");

        return Ok(ApiResponse<object>.SuccessResponse(isDeleted, "The section deleted successfully!"));
    }

    /// <summary>
    /// Delete all Sections (Delete All)
    /// </summary>
    /// <returns>True if all sections deleted successfully</returns>
    /// <response code="200">All sections deleted successfully</response>
    /// <response code="404">No sections to delete</response>
    [HttpDelete("all")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteAll()
    {
        var isDeleted = await _parkingSectionService.DeleteAllAsync();

        if (!isDeleted)
            return NotFound("No sections to delete");

        return Ok(ApiResponse<object>.SuccessResponse(isDeleted, "All sections deleted successfully!"));
    }

    /// <summary>
    /// Update a Section by ID (Update by ID)
    /// </summary>
    /// <param name="id">The ID of the section to update</param>
    /// <param name="updateRequest">The updated section data</param>
    /// <returns>The updated section wrapped in ParkingSectionResponseDto</returns>
    /// <response code="200">Section updated successfully</response>
    /// <response code="404">Section not found</response>
    [HttpPut]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ApiResponse<ParkingSectionResponseDto>>> Update(Guid id, UpdateParkingSectionRequestDto updateRequest)
    {
        var updatedSection = await _parkingSectionService.UpdateAsync(id, updateRequest);

        if (updatedSection is null)
            return NotFound("The section you want to update was not found!");

        return Ok(ApiResponse<ParkingSectionResponseDto>.SuccessResponse(updatedSection, "Section updated successfully!"));
    }
}
