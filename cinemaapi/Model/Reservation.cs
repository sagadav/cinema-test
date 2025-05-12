using System;
using System.Collections.Generic;

namespace cinemaapi.Model;

public partial class Reservation
{
    public int Id { get; set; }

    public int ScreeningId { get; set; }

    public int UserId { get; set; }

    public DateTime? PurchaseDate { get; set; }

    public virtual Screening Screening { get; set; } = null!;

    public virtual ICollection<SeatReserved> SeatReserveds { get; } = new List<SeatReserved>();

    public virtual User User { get; set; } = null!;
}
