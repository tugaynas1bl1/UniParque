namespace UniParque.Application.DTOs;

public class ParkingBranchResponseDto
{
    /// <summary>
    /// The unique ID of the parking branch.
    /// </summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid Id { get; set; }

    /// <summary>
    /// The name of the parking branch.
    /// </summary>
    /// <example>28 Mall Parking</example>
    public string BranchName { get; set; }
}

public class CreateParkingBranchRequestDto
{
    /// <summary>
    /// The name of the parking branch.
    /// </summary>
    /// <example>28 Mall Parking</example>
    public string BranchName { get; set; }
}

public class UpdateParkingBranchRequestDto
{
    /// <summary>
    /// The new name of the parking branch.
    /// </summary>
    /// <example>28 Mall Parking</example>
    public string BranchName { get; set; }
}
