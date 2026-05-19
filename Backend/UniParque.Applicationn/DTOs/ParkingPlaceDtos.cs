namespace UniParque.Application.DTOs;

public class ParkingPlaceResponseDto
{
    /// <summary>
    /// The unique ID of the parking place.
    /// </summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid Id { get; set; }

    /// <summary>
    /// The name or label of the parking place.
    /// </summary>
    /// <example>Place 1</example>
    public string PlaceName { get; set; }

    /// <summary>
    /// The name of the Subsection this place belongs to.
    /// </summary>
    /// <example>A1</example>
    public string SubSection { get; set; }
}

public class CreateParkingPlaceRequestDto
{
    /// <summary>
    /// The ID of the subsection that this place belongs to.
    /// </summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid SubSectionId { get; set; }

    /// <summary>
    /// The name or label of the parking place.
    /// </summary>
    /// <example>Place 1</example>
    public string PlaceName { get; set; }
}

public class UpdateParkingPlaceRequestDto
{
    /// <summary>
    /// The new name or label of the parking place.
    /// </summary>
    /// <example>A3</example>
    public string PlaceName { get; set; }
}