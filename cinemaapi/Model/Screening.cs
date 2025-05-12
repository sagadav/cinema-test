using System;
using System.Collections.Generic;

namespace cinemaapi.Model;

public partial class Screening
{
    public int Id { get; set; }

    public int MovieId { get; set; }

    public int AuditoriumId { get; set; }

    public DateTime ScreeningStamp { get; set; }

    public int Cost { get; set; }

    public virtual Auditorium Auditorium { get; set; } = null!;

    public virtual Movie Movie { get; set; } = null!;

    public virtual ICollection<Reservation> Reservations { get; } = new List<Reservation>();
}
