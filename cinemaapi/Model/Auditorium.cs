using System;
using System.Collections.Generic;

namespace cinemaapi.Model;

public partial class Auditorium
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public int SeatsNo { get; set; }

    public virtual ICollection<Screening> Screenings { get; } = new List<Screening>();

    public virtual ICollection<Seat> Seats { get; } = new List<Seat>();
}
