using System;
using System.Collections.Generic;

namespace cinemaapi.Model;

public partial class SeatReserved
{
    public int Id { get; set; }

    public int SeatId { get; set; }

    public int ReservationId { get; set; }

    public virtual Reservation Reservation { get; set; } = null!;

    public virtual Seat Seat { get; set; } = null!;
}
