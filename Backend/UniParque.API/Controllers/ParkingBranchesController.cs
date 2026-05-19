using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniParque.Application.Common;
using UniParque.Application.DTOs;
using UniParque.Application.Services;

[Route("api/[controller]")]
[ApiController]
public class ParkingBranchesController : ControllerBase
{
    private readonly IParkingBranchService _parkingBranchService;

    public ParkingBranchesController(IParkingBranchService parkingBranchService)
    {
        _parkingBranchService = parkingBranchService;
    }

    /// <summary>
    /// Create a new Parking Branch
    /// </summary>
    /// <param name="createdBranchRequestDto">The payload used to add a new branch</param>
    /// <returns>The created branch wrapped in ParkingBranchResponseDto</returns>
    /// <response code="201">The branch successfully added</response>
    /// <response code="400">Request body is invalid</response>
    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ApiResponse<ParkingBranchResponseDto>>> Create(
        [FromBody] CreateParkingBranchRequestDto createdBranchRequestDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var createdBranch = await _parkingBranchService.CreateAsync(createdBranchRequestDto);

        return CreatedAtAction(nameof(GetById), new { id = createdBranch.Id },
            ApiResponse<ParkingBranchResponseDto>.SuccessResponse(createdBranch, "Parking branch created successfully!"));
    }

    /// <summary>
    /// Get a Parking Branch by ID (Get by ID)
    /// </summary>
    /// <param name="id">The ID of the branch to retrieve</param>
    /// <returns>The branch wrapped in ParkingBranchResponseDto</returns>
    /// <response code="200">Branch found</response>
    /// <response code="404">Branch not found</response>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ParkingBranchResponseDto>>> GetById(Guid id)
    {
        var branch = await _parkingBranchService.GetByIdAsync(id);

        if (branch is null)
            return NotFound("Parking branch not found!");

        return Ok(ApiResponse<ParkingBranchResponseDto>.SuccessResponse(branch));
    }

    /// <summary>
    /// Get all Parking Branches (Get All)
    /// </summary>
    /// <returns>All branches wrapped in ParkingBranchResponseDto</returns>
    /// <response code="200">Branches found</response>
    /// <response code="404">No branches found</response>
    [HttpGet("all")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ParkingBranchResponseDto>>>> GetAll()
    {
        var branches = await _parkingBranchService.GetAllAsync();

        if (branches is null)
            return NotFound("No branches found");

        return Ok(ApiResponse<IEnumerable<ParkingBranchResponseDto>>.SuccessResponse(branches));
    }

    /// <summary>
    /// Delete a Parking Branch by ID (Delete by ID)
    /// </summary>
    /// <param name="id">The ID of the branch to delete</param>
    /// <returns>True if deleted successfully</returns>
    /// <response code="200">Branch deleted successfully</response>
    /// <response code="404">Branch not found or could not be deleted</response>
    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
    {
        var isDeleted = await _parkingBranchService.DeleteAsync(id);

        if (!isDeleted)
            return NotFound("The branch could not be deleted");

        return Ok(ApiResponse<object>.SuccessResponse(isDeleted, "The branch deleted successfully!"));
    }

    /// <summary>
    /// Delete all Parking Branches (Delete All)
    /// </summary>
    /// <returns>True if all branches deleted successfully</returns>
    /// <response code="200">All branches deleted successfully</response>
    /// <response code="404">No branches to delete</response>
    [HttpDelete("all")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteAll()
    {
        var isDeleted = await _parkingBranchService.DeleteAllAsync();

        if (!isDeleted)
            return NotFound("No branches to delete");

        return Ok(ApiResponse<object>.SuccessResponse(isDeleted, "All branches deleted successfully!"));
    }

    /// <summary>
    /// Get paged branches
    /// </summary>
    /// <param name="queryParams"></param>
    /// <returns>Returned branches wrapped in ParkingBranchResponseDto</returns>
    /// <response code="200">Operation executed successfully</response>
    /// <response code="404">Branches not found</response>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ParkingBranchResponseDto>>>> GetPaged(
        [FromQuery] ParkingBranchQueryParams queryParams)
    {
        var result = await _parkingBranchService.GetPagedAsync(queryParams);
        return Ok(ApiResponse<PagedResult<ParkingBranchResponseDto>>.SuccessResponse(result));
    }

    /// <summary>
    /// Get Fully Branch (with sections, subsections and places)
    /// </summary>
    /// <param name="queryParams"></param>
    /// <returns>Returned branches wrapped in ParkingBranchResponseDto</returns>
    /// <response code="200">Operation executed successfully</response>
    /// <response code="404">Branches not found</response>
    [HttpGet("full-layout/{branchId}")]
    public async Task<ActionResult<ApiResponse<ParkingBranchFullLayoutDto>>> GetFullLayout(Guid branchId)
    {
        var result = await _parkingBranchService.GetFullLayoutByIdAsync(branchId);
        return Ok(ApiResponse<ParkingBranchFullLayoutDto>.SuccessResponse(result));
    }
}