using UniParque_Domain.Entities;

namespace UniParque.Application.DTOs;

public class ReservationResponseDto
{
    /// <summary>
    /// Unique ID of the reservation
    /// </summary>
    /// <example>d290f1ee-6c54-4b01-90e6-d701748f0851</example>
    public Guid Id { get; set; }

    /// <summary>
    /// Full name or identifier of the user
    /// </summary>
    /// <example>John Doe</example>
    public string User { get; set; } = string.Empty;

    /// <summary>
    /// Branch of the parking facility
    /// </summary>
    /// <example>Main Branch</example>
    public string Branch { get; set; } = string.Empty;

    /// <summary>
    /// Section within the branch
    /// </summary>
    /// <example>Section A</example>
    public string Section { get; set; } = string.Empty;

    /// <summary>
    /// Sub-section or zone within the section
    /// </summary>
    /// <example>Sub-section 1</example>
    public string SubSection { get; set; } = string.Empty;

    /// <summary>
    /// Specific parking place identifier
    /// </summary>
    /// <example>Place 42</example>
    public string Place { get; set; } = string.Empty;

    /// <summary>
    /// Place occupied or not
    /// </summary>
    /// <example>false</example>

    public bool IsPlaceOccupied { get; set; }

    /// <summary>
    /// Car number/license plate
    /// </summary>
    /// <example>90-AB-123</example>
    public string CarNumber { get; set; } = string.Empty;

    /// <summary>
    /// Expected arrival time of the car
    /// </summary>
    /// <example>2026-03-04T15:30:00+04:00</example>
    public DateTimeOffset EstimatedArrivalTime { get; set; }

    /// <summary>
    /// Total price of the reservation
    /// </summary>
    /// <example>15.50</example>
    public decimal Price { get; set; }

    /// <summary>
    /// Checked if current user reserved
    /// </summary>
    /// <example>true</example>
    public bool isReservedByMe { get; set; }

    /// <summary>
    /// Reservation Status
    /// </summary>
    /// <example>Active</example>
    public ReservationStatus Status { get; set; }

    public DateTimeOffset? CreatedAt { get; set; }
}

public class CreateReservationRequestDto
{
    /// <summary>
    /// ID of the parking place to reserve
    /// </summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid PlaceId { get; set; }

    /// <summary>
    /// Expected arrival time of the car
    /// </summary>
    /// <example>2026-03-04T15:30:00+04:00</example>
    public DateTimeOffset EstimatedArrivalTime { get; set; }

    /// <summary>
    /// Car number/license plate
    /// </summary>
    /// <example>90-AB-123</example>
    public string CarNumber { get; set; } = string.Empty;
}

public class CalculatePriceRequestDto
{
    /// <summary>
    /// Expected arrival time of the car
    /// </summary>
    /// <example>2026-03-04T15:30:00+04:00</example>
    public DateTimeOffset EstimatedArrivalTime { get; set; }
}

public class CreateReservationRequestWithSpecificUserDto
{
    /// <summary>
    /// ID of the user making the reservation
    /// </summary>
    /// <example>d290f1ee-6c54-4b01-90e6-d701748f0851</example>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// ID of the parking place to reserve
    /// </summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid PlaceId { get; set; }

    /// <summary>
    /// Expected arrival time of the car
    /// </summary>
    /// <example>2026-03-04T15:30:00+04:00</example>
    public DateTimeOffset EstimatedArrivalTime { get; set; }

    /// <summary>
    /// Car number/license plate
    /// </summary>
    /// <example>90-AB-123</example>
    public string CarNumber { get; set; } = string.Empty;
}

public class CreateReservationWithCardRequestDto
{
    /// <summary>
    /// Card number used for the transaction (required for deposit or withdraw operations)
    /// </summary>
    /// <example>4098584487322842</example>
    public string CardNumber { get; set; } = string.Empty;

    public CreateReservationRequestDto Reservation { get; set; }
}

public class UpdateReservationRequestDto
{
    /// <summary>
    /// ID of the parking place to update
    /// </summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid PlaceId { get; set; }

    /// <summary>
    /// Updated car number/license plate
    /// </summary>
    /// <example>90-AB-456</example>
    public string CarNumber { get; set; } = string.Empty;
}
