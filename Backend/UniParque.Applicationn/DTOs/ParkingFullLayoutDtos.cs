using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniParque_Domain.Entities;

namespace UniParque.Application.DTOs;

public class ParkingBranchFullLayoutDto
{
    public Guid Id { get; set; }
    public string BranchName { get; set; }
    public List<SectionDto> Sections { get; set; }
}

public class SectionDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<SubSectionDto> SubSections { get; set; }
}

public class SubSectionDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<PlaceDto> Places { get; set; }
}

public class PlaceDto
{
    public Guid Id { get; set; }
    public bool IsReserved { get; set; }
    public bool IsOccupied { get; set; }
    public bool IsReservedByMe { get; set; }
}

public class ReservationDto
{
    public Guid Id { get; set; }
    public AppUser User { get; set; }
}
