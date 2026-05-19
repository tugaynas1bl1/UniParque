namespace UniParque.Application.DTOs;

public class ParkingSectionResponseDto
{
    /// <summary>
    /// The unique ID of the parking section.
    /// </summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid Id { get; set; }

    /// <summary>
    /// The name or label of the parking section.
    /// </summary>
    /// <example>A</example>
    public string Section { get; set; }

    /// <summary>
    /// The name of the branch this section belongs to.
    /// </summary>
    /// <example>28 Mall Parking</example>
    public string Branch { get; set; }
}

public class CreateParkingSectionRequestDto
{
    /// <summary>
    /// The ID of the branch that this section belongs to.
    /// </summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid BranchId { get; set; }

    /// <summary>
    /// The name or label of the parking section.
    /// </summary>
    /// <example>A</example>
    public string Section { get; set; }
}

public class UpdateParkingSectionRequestDto
{
    /// <summary>
    /// The new name or label of the parking section.
    /// </summary>
    /// <example>B</example>
    public string Section { get; set; }
}
