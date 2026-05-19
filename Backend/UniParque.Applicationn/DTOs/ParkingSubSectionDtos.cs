namespace UniParque.Application.DTOs;

public class ParkingSubSectionResponseDto
{
    /// <summary>
    /// The unique ID of the parking subsection.
    /// </summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid Id { get; set; }

    /// <summary>
    /// The name or label of the parking subsection.
    /// </summary>
    /// <example>A1</example>
    public string SubSection { get; set; }

    /// <summary>
    /// The name of the Section this subsection belongs to.
    /// </summary>
    /// <example>A</example>
    public string Section { get; set; }
}

public class CreateParkingSubSectionRequestDto
{
    /// <summary>
    /// The ID of the section that this subsection belongs to.
    /// </summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid SectionId { get; set; }

    /// <summary>
    /// The name or label of the parking subsection.
    /// </summary>
    /// <example>A1</example>
    public string SubSection {  get; set; }
}

public class UpdateParkingSubSectionRequestDto
{
    /// <summary>
    /// The new name or label of the parking subsection.
    /// </summary>
    /// <example>A3</example>
    public string SubSection { get; set; }
}
