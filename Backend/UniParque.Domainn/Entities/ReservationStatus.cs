using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniParque_Domain.Entities;

public enum ReservationStatus
{
    Active,
    Completed,
    CheckedIn,
    Expired,
    Cancelled
}
