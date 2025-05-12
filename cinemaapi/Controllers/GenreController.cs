using cinemaapi.Model;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace cinemaapi.Controllers
{
    public class GenreController : Controller
    {
        private readonly CinemaContext _context;
        public GenreController(CinemaContext context)
        {
            _context = context;
        }

        [HttpGet("GetGenres")]
        public IActionResult GetGenres()
        {
            var genres = _context.Genres.Select(x => new { x.Id, x.Title }).ToList();

            var json = System.Text.Json.JsonSerializer.Serialize(genres, new JsonSerializerOptions() { ReferenceHandler = ReferenceHandler.IgnoreCycles });
            return Ok(json);
        }
    }
}
