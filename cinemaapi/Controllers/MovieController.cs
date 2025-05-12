using cinemaapi.DTO;
using cinemaapi.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace cinemaapi.Controllers
{
    public class MovieController : Controller
    {
        private readonly CinemaContext _context;
        public MovieController(CinemaContext context)
        {
            _context = context;
        }

        [HttpGet("GetMovies")]
        public IActionResult GetMovieTitle()
        {
            var movies = _context.Movies.Select(x => new { x.Id, x.Title }).ToList();

            var json = System.Text.Json.JsonSerializer.Serialize(movies, new JsonSerializerOptions() { ReferenceHandler = ReferenceHandler.IgnoreCycles });
            return Ok(json);
        }

        [HttpGet("Movie/{id}")]
        public IActionResult GetMovieData(int id)
        {
            var movie = _context.MovieGenres
                .Include(x => x.Movie)
                .Include(x => x.Genre)
                .Where(x => x.MovieId == id)
                .Select(x => new MovieGenreDto()
                {
                    Id = x.Id,
                    MovieId = x.MovieId,
                    GenreId = x.GenreId,
                    Genre = new GenreDto()
                    {
                        Id = x.Genre.Id,
                        Title = x.Genre.Title,
                    },
                    Movie = new MovieDto(x.Movie)
                })
                .OrderBy(x => x.Id);
            var json = System.Text.Json.JsonSerializer.Serialize(movie, new JsonSerializerOptions() { ReferenceHandler = ReferenceHandler.IgnoreCycles });
            return Ok(json);
        }

        [HttpGet("Movie/{id}/Dates")]
        public IActionResult GetMovieScreeningDates(int id)
        {
            // && x.ScreeningStamp >= DateTime.Now
            var dates = _context.Screenings
                .Where(x => x.MovieId == id)
                .Select(x => x.ScreeningStamp)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            var json = System.Text.Json.JsonSerializer.Serialize(dates);
            return Ok(json);
        }

        [HttpPost("AddMovie")]
        public async Task<IActionResult> AddMovieToDb([FromForm] MovieRequest request)
        {
            if (request == null)
                return BadRequest("Error!");

            byte[] posterBytes;
            using (var memoryStream = new MemoryStream())
            {
                await request.Poster.CopyToAsync(memoryStream);
                posterBytes = memoryStream.ToArray();
            }

            var movie = new Movie()
            {
                Title = request.Title,
                MovieDescription = request.MovieDescription,
                Director = request.Director,
                Cast = request.Cast,
                DurationMin = request.DurationMin,
                Country = request.Country,
                PremierDate = request.PremierDate,
                Poster = posterBytes
            };

            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();
            Debug.WriteLine("MovieGenre: " + request.Genres);

            string[] movieGenres = request.Genres.Split(',');

            foreach (var genre in movieGenres)
            {
                MovieGenre movieGenre = new MovieGenre()
                {
                    GenreId = int.Parse(genre),
                    MovieId = movie.Id
                };

                _context.MovieGenres.Add(movieGenre);
            }

            _context.SaveChanges();

            return Ok("Добавлен новый фильм");
        }

        public class MovieRequest
        {
            public string Title { get; set; }
            public string MovieDescription { get; set; }
            public string Director { get; set; }
            public string Cast { get; set; }
            public int DurationMin { get; set; }
            public string Country { get; set; }
            public DateTime PremierDate { get; set; }
            public IFormFile Poster { get; set; }
            public string Genres { get; set; }
        }

        [HttpDelete("DeleteMovie/{id}")]
        public IActionResult DeleteMovie(int id)
        {
            var movie = _context.Movies.Where(x => x.Id == id).FirstOrDefault();
            _context.Movies.Remove(movie);
            _context.SaveChanges();
            return Ok("Deleted");
        }
    }
}
