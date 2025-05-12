using System;
using System.Collections.Generic;

namespace cinemaapi.Model;

public partial class Seat
{
    public int Id { get; set; }

    public int SeatRow { get; set; }

    public int SeatNum { get; set; }

    public int AuditoriumId { get; set; }

    public virtual Auditorium Auditorium { get; set; } = null!;

    public virtual ICollection<SeatReserved> SeatReserveds { get; } = new List<SeatReserved>();
}
