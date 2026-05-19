using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using UniParque.Application.Common;
using UniParque.Application.DTOs;
using UniParque.Application.Hubs;
using UniParque.Application.Services;

namespace UniParque.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReservationsController : ControllerBase
{
    private readonly IReservationService _reservationService;
    private readonly IHubContext<ReservationHub> _hubContext;

    public ReservationsController(IReservationService reservationService, IHubContext<ReservationHub> hubContext)
    {
        _reservationService = reservationService;
        _hubContext = hubContext;
    }

    /// <summary>
    /// Create a new Parking Reservation
    /// </summary>
    /// <param name="createdReservationRequestDto">The payload used to add a new reservation</param>
    /// <returns>The created reservation wrapped in ReservationResponseDto</returns>
    /// <response code="201">The reservation successfully added</response>
    /// <response code="400">Request body is invalid</response>
    [HttpPost("create")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<ReservationResponseDto>>> Create(
        [FromBody] CreateReservationWithCardRequestDto createdReservationRequestDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var createdReservation = await _reservationService.CreateAsync(createdReservationRequestDto);

        var hubContext = _hubContext;
        await hubContext.Clients.All.SendAsync("ReservationUpdated");

        return CreatedAtAction(nameof(GetById), new { id = createdReservation.Id },
            ApiResponse<ReservationResponseDto>.SuccessResponse(createdReservation, "Parking reserved successfully!"));
    }

    [HttpPost("create-by-balance")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<ReservationResponseDto>>> CreateByBalance(
        [FromBody] CreateReservationRequestDto createdReservationRequestDto)
    {

        var createdReservation = await _reservationService.CreateByBalanceAsync(createdReservationRequestDto);

        var hubContext = _hubContext;
        await hubContext.Clients.All.SendAsync("ReservationUpdated");

        return CreatedAtAction(nameof(GetById), new { id = createdReservation.Id },
            ApiResponse<ReservationResponseDto>.SuccessResponse(createdReservation, "Parking reserved successfully!"));
    }

    [HttpPost("create-by-user-id")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<ReservationResponseDto>>> CreateWithUserId(
        [FromBody] CreateReservationRequestWithSpecificUserDto createdReservationRequestDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var createdReservation = await _reservationService.CreateWithSpecificUserAsync(createdReservationRequestDto);

        return CreatedAtAction(nameof(GetById), new { id = createdReservation.Id },
            ApiResponse<ReservationResponseDto>.SuccessResponse(createdReservation, "Parking reserved successfully!"));
    }
    /// <summary>
    /// Get a Parking Reservation by ID (Get by ID)
    /// </summary>
    /// <param name="id">The ID of the reservation to retrieve</param>
    /// <returns>The reservation wrapped in ReservationResponseDto</returns>
    /// <response code="200">Reservation found</response>
    /// <response code="404">Reservation not found</response>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ReservationResponseDto>>> GetById(Guid id)
    {
        var reservation = await _reservationService.GetByIdAsync(id);

        if (reservation is null)
            return NotFound("Parking reservation not found!");

        return Ok(ApiResponse<ReservationResponseDto>.SuccessResponse(reservation));
    }

    /// <summary>
    /// Get all Parking Reservations (Get All)
    /// </summary>
    /// <returns>All reservations wrapped in ReservationResponseDto</returns>
    /// <response code="200">Reservations found</response>
    /// <response code="404">No reservations found</response>
    [HttpGet("all")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ReservationResponseDto>>>> GetAll()
    {
        var reservations = await _reservationService.GetAllAsync();

        if (reservations is null)
            return NotFound("No reservations found");

        return Ok(ApiResponse<IEnumerable<ReservationResponseDto>>.SuccessResponse(reservations));
    }

    /// <summary>
    /// Delete a Parking Reservation by ID (Delete by ID)
    /// </summary>
    /// <param name="id">The ID of the reservation to delete</param>
    /// <returns>True if deleted successfully</returns>
    /// <response code="200">Reservation deleted successfully</response>
    /// <response code="404">Reservation not found or could not be deleted</response>
    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
    {
        var isDeleted = await _reservationService.DeleteAsync(id);

        if (!isDeleted)
            return NotFound("The reservation could not be deleted");

        return Ok(ApiResponse<object>.SuccessResponse(isDeleted, "The reservation deleted successfully!"));
    }

    /// <summary>
    /// Delete all Parking Reservations (Delete All)
    /// </summary>
    /// <returns>True if all reservations deleted successfully</returns>
    /// <response code="200">All reservations deleted successfully</response>
    /// <response code="404">No reservations to delete</response>
    [HttpDelete("all")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteAll()
    {
        var isDeleted = await _reservationService.DeleteAllAsync();

        if (!isDeleted)
            return NotFound("No reservations to delete");

        return Ok(ApiResponse<object>.SuccessResponse(isDeleted, "All reservations deleted successfully!"));
    }

    /// <summary>
    /// Update a Parking Reservation by ID (Update by ID)
    /// </summary>
    /// <param name="id">The ID of the reservation to update</param>
    /// <param name="updateRequest">The updated reservation data</param>
    /// <returns>The updated reservation wrapped in ReservationResponseDto</returns>
    /// <response code="200">Reservation updated successfully</response>
    /// <response code="404">Reservation not found</response>
    [HttpPut]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ApiResponse<ReservationResponseDto>>> Update(Guid id, UpdateReservationRequestDto updateRequest)
    {
        var updatedReservation = await _reservationService.UpdateAsync(id, updateRequest);

        if (updatedReservation is null)
            return NotFound("The reservation you want to update was not found!");

        return Ok(ApiResponse<ReservationResponseDto>.SuccessResponse(updatedReservation, "Reservation updated successfully!"));
    }

    /// <summary>
    /// Confirm your arrival and take your place
    /// </summary>
    /// <param name="reservationId">The ID of the reservation to confirm the arrival</param>
    /// <returns>True if arrival confirmed</returns>
    /// <response code="200">Reservation arrival confirmed</response>
    /// <response code="404">Reservation not found</response>
    [HttpPut("confirm-arrival/{reservationId}")]
    [Authorize(Policy = "UserOrAbove")]
    public async Task<ActionResult<ApiResponse<bool>>> ConfirmArrival(Guid reservationId)
    {
        var confirmedReservation = await _reservationService.ConfirmArrivalAsync(reservationId);

        if (!confirmedReservation)
            return BadRequest("The reservation confirmation went wrong!");

        var hubContext = _hubContext;
        await hubContext.Clients.All.SendAsync("ReservationUpdated");

        return Ok(ApiResponse<object>.SuccessResponse(confirmedReservation, "Arrival confirmed! Place is taken!"));
    }

    [HttpDelete("end-session/{reservationId}")]
    [Authorize(Policy = "UserOrAbove")]
    public async Task<ActionResult<ApiResponse<bool>>> EndReservationSessionAndLeave(Guid reservationId)
    {
        var endedReservation = await _reservationService.EndReservationSessionAndLeaveAsync(reservationId);

        if (!endedReservation)
            return BadRequest("The reservation session went wrong!");

        var hubContext = _hubContext;
        await hubContext.Clients.All.SendAsync("ReservationUpdated");

        return Ok(ApiResponse<object>.SuccessResponse(endedReservation, "You ended the reservation session successfully"));
    }

    [HttpDelete("cancel-reservation/{reservationId}")]
    [Authorize(Policy = "UserOrAbove")]
    public async Task<ActionResult<ApiResponse<bool>>> CancelReservation(Guid reservationId)
    {
        var cancelledReservation = await _reservationService.CancelReservationAsync(reservationId);

        if (!cancelledReservation)
            return BadRequest("The reservation session went wrong!");    
         
        var hubContext = _hubContext;
        await hubContext.Clients.All.SendAsync("ReservationUpdated");

        return Ok(ApiResponse<object>.SuccessResponse(cancelledReservation, "You cancelled the reservation session successfully!"));
    }

    [HttpPost("calculate-price")]
    [Authorize(Policy = "UserOrAbove")]
    public async Task<ActionResult<ApiResponse<decimal>>> CalculatePrice([FromBody] CalculatePriceRequestDto request)
    {
        var price = await _reservationService.CalculatePriceAsync(request);
        return Ok(ApiResponse<decimal>.SuccessResponse(price));
    }

    /// <summary>
    /// Get Parking Reservations by user ID
    /// </summary>
    /// <param name="userId">gets userId</param>
    /// <returns>All reservations wrapped in ReservationResponseDto</returns>
    /// <response code="200">Reservations found</response>
    /// <response code="404">No reservations found</response>
    [HttpGet("get-by-user/{userId}")]
    [Authorize(Policy="AdminOrManager")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ReservationResponseDto>>>> GetByUserId(string userId)
    {
        var reservations = await _reservationService.GetByUserIdAsync(userId);

        if (reservations is null)
            return NotFound("No reservations found");

        return Ok(ApiResponse<IEnumerable<ReservationResponseDto>>.SuccessResponse(reservations));
    }

    /// <summary>
    /// Get Parking Reservations by current user.
    /// </summary>
    /// <returns>All reservations wrapped in ReservationResponseDto</returns>
    /// <response code="200">Reservations found</response>
    /// <response code="404">No reservations found</response>
    [HttpGet("get-by-current-user")]
    [Authorize(Policy = "UserOrAbove")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ReservationResponseDto>>>> GetByCurrentUser()
    {
        var reservations = await _reservationService.GetByCurrentUser();

        if (reservations is null)
            return NotFound("No reservations found");

        return Ok(ApiResponse<IEnumerable<ReservationResponseDto>>.SuccessResponse(reservations));
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ReservationResponseDto>>>> GetPaged(
        [FromQuery] ReservationQueryParams queryParams)
    {
        var result = await _reservationService.GetPagedAsync(queryParams);
        return Ok(ApiResponse<PagedResult<ReservationResponseDto>>.SuccessResponse(result));
    }

    [HttpGet("count-active")]
    public async Task<ActionResult<ApiResponse<PagedResult<int>>>> GetCountActive()
    {
        var result = await _reservationService.GetActiveReservationsCountAsync();
        return Ok(ApiResponse<int>.SuccessResponse(result));
    }
}
