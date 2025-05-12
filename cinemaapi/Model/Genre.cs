using System;
using System.Collections.Generic;

namespace cinemaapi.Model;

public partial class Genre
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public virtual ICollection<MovieGenre> MovieGenres { get; } = new List<MovieGenre>();
}
