using cinemaapi.Model;
using Microsoft.EntityFrameworkCore;

namespace cinemaapi.DTO
{
    public class MovieDto
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


        public MovieDto(Movie movie)
        {
            Id = movie.Id;
            Title = movie.Title;
            MovieDescription = movie.MovieDescription;
            Director = movie.Director;
            Cast = movie.Cast;
            DurationMin = movie.DurationMin;
            Poster = movie.Poster;
            PremierDate = movie.PremierDate;
            Country = movie.Country;
        }
    }
}
