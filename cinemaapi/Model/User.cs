﻿using System;
using System.Collections.Generic;

namespace cinemaapi.Model;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Role { get; set; } = null!;

    public virtual ICollection<Reservation> Reservations { get; } = new List<Reservation>();
}
