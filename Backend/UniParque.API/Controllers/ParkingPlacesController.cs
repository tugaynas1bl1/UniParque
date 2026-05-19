using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UniParque.Application.Common;
using UniParque.Application.DTOs;
using UniParque.Application.Services;

namespace UniParque.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ParkingPlacesController : ControllerBase
{
    private readonly IParkingPlaceService _parkingPlaceService;

    public ParkingPlacesController(IParkingPlaceService parkingPlaceService)
    {
        _parkingPlaceService = parkingPlaceService;
    }

    /// <summary>
    /// Create a new Parking Place
    /// </summary>
    /// <param name="createdPlaceRequestDto">The payload used to add a new place</param>
    /// <returns>The created place wrapped in ParkingPlaceResponseDto</returns>
    /// <response code="201">The place successfully added</response>
    /// <response code="400">Request body is invalid</response>
    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ApiResponse<ParkingPlaceResponseDto>>> Create(
        [FromBody] CreateParkingPlaceRequestDto createdPlaceRequestDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var createdPlace = await _parkingPlaceService.CreateAsync(createdPlaceRequestDto);

        return CreatedAtAction(nameof(GetById), new { id = createdPlace.Id },
            ApiResponse<ParkingPlaceResponseDto>.SuccessResponse(createdPlace, "Parking place created successfully!"));
    }

    /// <summary>
    /// Get a Place by ID (Get by ID)
    /// </summary>
    /// <param name="id">The ID of the place to retrieve</param>
    /// <returns>The place wrapped in ParkingPlaceResponseDto</returns>
    /// <response code="200">Place found</response>
    /// <response code="404">Place not found</response>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ParkingPlaceResponseDto>>> GetById(Guid id)
    {
        var place = await _parkingPlaceService.GetByIdAsync(id);

        if (place is null)
            return NotFound("Parking place not found!");

        return Ok(ApiResponse<ParkingPlaceResponseDto>.SuccessResponse(place));
    }

    /// <summary>
    /// Get all Places (Get All)
    /// </summary>
    /// <returns>All places wrapped in ParkingPlaceResponseDto</returns>
    /// <response code="200">Places found</response>
    /// <response code="404">No places found</response>
    [HttpGet("all")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ParkingPlaceResponseDto>>>> GetAll()
    {
        var places = await _parkingPlaceService.GetAllAsync();

        if (places is null)
            return NotFound("No places found");

        return Ok(ApiResponse<IEnumerable<ParkingPlaceResponseDto>>.SuccessResponse(places));
    }

    /// <summary>
    /// Get places by Subsection ID (Get by Subsection ID)
    /// </summary>
    /// <param name="subSectionId">The ID of the subsection</param>
    /// <returns>All places of the subsection wrapped in ParkingPlaceResponseDto</returns>
    /// <response code="200">Places found</response>
    /// <response code="404">No places found for this subsection</response>
    [HttpGet("places/{subSectionId}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ParkingPlaceResponseDto>>>> GetBySubSectionId(Guid subSectionId)
    {
        var places = await _parkingPlaceService.GetBySubSectionIdAsync(subSectionId);

        if (places is null)
            return NotFound("No places found for this branch");

        return Ok(ApiResponse<IEnumerable<ParkingPlaceResponseDto>>.SuccessResponse(places));
    }

    /// <summary>
    /// Delete a Parking Place (Delete by ID)
    /// </summary>
    /// <param name="id">The ID of the place to delete</param>
    /// <returns>True if deleted successfully</returns>
    /// <response code="200">Place deleted successfully</response>
    /// <response code="404">Place not found or could not be deleted</response>
    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
    {
        var isDeleted = await _parkingPlaceService.DeleteAsync(id);

        if (!isDeleted)
            return NotFound("The place could not be deleted");

        return Ok(ApiResponse<object>.SuccessResponse(isDeleted, "The place deleted successfully!"));
    }

    /// <summary>
    /// Delete all Parking Places (Delete All)
    /// </summary>
    /// <returns>True if all places deleted successfully</returns>
    /// <response code="200">All places deleted successfully</response>
    /// <response code="404">No places to delete</response>
    [HttpDelete("all")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteAll()
    {
        var isDeleted = await _parkingPlaceService.DeleteAllAsync();

        if (!isDeleted)
            return NotFound("No places to delete");

        return Ok(ApiResponse<object>.SuccessResponse(isDeleted, "All places deleted successfully!"));
    }

    /// <summary>
    /// Update a Parking Place (Update by ID)
    /// </summary>
    /// <param name="id">The ID of the place to update</param>
    /// <param name="updateRequest">The updated place data</param>
    /// <returns>The updated place wrapped in ParkingPlaceResponseDto</returns>
    /// <response code="200">Place updated successfully</response>
    /// <response code="404">Place not found</response>
    [HttpPut]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ApiResponse<ParkingPlaceResponseDto>>> Update(Guid id, UpdateParkingPlaceRequestDto updateRequest)
    {
        var updatedPlace = await _parkingPlaceService.UpdateAsync(id, updateRequest);

        if (updatedPlace is null)
            return NotFound("The place you want to update was not found!");

        return Ok(ApiResponse<ParkingPlaceResponseDto>.SuccessResponse(updatedPlace, "Place updated successfully!"));
    }
}
