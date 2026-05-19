using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniParque.Application.DTOs;

public class ParkingBranchQueryParams
{
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 10;
    public string? Sort { get; set; }
    public string SortDirection { get; set; } = "desc";
    public string? Search {  get; set; }

    public void Validate()
    {
        if (Page < 1) Page = 1;
        if (Size < 1) Size = 1;
        if (Size > 100) Size = 100;
        if (string.IsNullOrEmpty(SortDirection)) SortDirection = "desc";
        SortDirection = SortDirection.ToLower();
        if (SortDirection != "asc" && SortDirection != "desc") SortDirection = "desc";
    }
}

public class ParkingSectionQueryParams
{
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 10;
    public string? Sort { get; set; }
    public string SortDirection { get; set; } = "desc";
    public string? Search { get; set; }
    public int? BranchId { get; set; }

    public void Validate()
    {
        if (Page < 1) Page = 1;
        if (Size < 1) Size = 1;
        if (Size > 100) Size = 100;
        if (string.IsNullOrEmpty(SortDirection)) SortDirection = "desc";
        SortDirection = SortDirection.ToLower();
        if (SortDirection != "asc" && SortDirection != "desc") SortDirection = "desc";
    }
}

public class ReservationQueryParams
{
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 10;
    public string? Sort { get; set; }
    public string SortDirection { get; set; } = "desc";
    public string? Search { get; set; }
    public string? BranchId { get; set; }
    public string? SectionId { get; set; }
    public string? SubSectionId { get; set; }
    public string? PlaceId { get; set; }
    public string? UserId { get; set; }
    public string? Status { get; set; }


    public void Validate()
    {
        if (Page < 1) Page = 1;
        if (Size < 1) Size = 1;
        if (Size > 100) Size = 100;
        if (string.IsNullOrEmpty(SortDirection)) SortDirection = "desc";
        SortDirection = SortDirection.ToLower();
        if (SortDirection != "asc" && SortDirection != "desc") SortDirection = "desc";
    }
}
