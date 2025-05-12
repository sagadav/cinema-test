using cinemaapi.Model;

namespace cinemaapi.DTO
{
    public class MovieGenreDto
    {
        public int Id { get; set; }

        public int MovieId { get; set; }

        public int GenreId { get; set; }

        public virtual GenreDto Genre { get; set; } = null!;

        public virtual MovieDto Movie { get; set; } = null!;
    }
}
