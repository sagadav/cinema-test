using System;
using System.Collections.Generic;

namespace cinemaapi.Model;

public partial class Movie
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string MovieDescription { get; set; } = null!;

    public string Director { get; set; } = null!;

    public string Cast { get; set; } = null!;

    public int DurationMin { get; set; }

    public byte[]? Poster { get; set; }

    public DateTime PremierDate { get; set; }

    public string Country { get; set; } = null!;

    public virtual ICollection<MovieGenre> MovieGenres { get; } = new List<MovieGenre>();

    public virtual ICollection<Screening> Screenings { get; } = new List<Screening>();
}
