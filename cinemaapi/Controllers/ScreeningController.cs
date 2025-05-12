using cinemaapi.DTO;
using cinemaapi.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.AspNetCore.Cors;
using System.Diagnostics;

namespace cinemaapi.Controllers
{


    public class ScreeningController : Controller
    {
        private readonly CinemaContext _context;
        public ScreeningController(CinemaContext context)
        {
            _context = context;
        }
        
        [HttpGet("Schedule")]
        public IActionResult GetScheduleData(DateTime date)
        {
            var schedule = _context.Screenings
                .Include(x => x.Movie)
                .Include(x => x.Auditorium)
                .Select(x => new ScreeningDto()
                {
                    Id = x.Id,
                    ScreeningStamp = x.ScreeningStamp,
                    MovieId = x.MovieId,
                    AuditoriumId = x.AuditoriumId,
                    Cost = x.Cost,
                    AuditoriumDto = new AuditoriumDto()
                    {
                        Id = x.AuditoriumId,
                        SeatsNo = x.Auditorium.SeatsNo,
                        Title = x.Auditorium.Title,
                    },
                    MovieDto = new MovieDto(x.Movie)
                })
                .OrderBy(x => x.Id)
                .ToList()
                .FindAll(x => x.ScreeningStamp.ToString("yyyy-MM-dd") == date.ToString("yyyy-MM-dd"));

            var json = System.Text.Json.JsonSerializer.Serialize(schedule, new JsonSerializerOptions() { ReferenceHandler = ReferenceHandler.IgnoreCycles });
            return Ok(json);
        }

        [HttpGet("Screening/{id}")]
        public IActionResult GetScreeningData(int id)
        {
            var schedule = _context.Screenings
                .Include(x => x.Movie)
                .Include(x => x.Auditorium)
                .Where(x => x.Id == id)
                .Select(x => new ScreeningDto()
                {
                    Id = x.Id,
                    ScreeningStamp = x.ScreeningStamp,
                    MovieId = x.MovieId,
                    AuditoriumId = x.AuditoriumId,
                    Cost = x.Cost,
                    AuditoriumDto = new AuditoriumDto()
                    {
                        Id = x.AuditoriumId,
                        SeatsNo = x.Auditorium.SeatsNo,
                        Title = x.Auditorium.Title,
                    },
                    MovieDto = new MovieDto(x.Movie)
                })
                .OrderBy(x => x.Id)
                .ToList();

            var json = System.Text.Json.JsonSerializer.Serialize(schedule, new JsonSerializerOptions() { ReferenceHandler = ReferenceHandler.IgnoreCycles });
            return Ok(json);
        }

        [HttpGet("ScheduleDates")]
        public IActionResult GetScheduleDates()
        {
            var dates = _context.Screenings.Select(x => x.ScreeningStamp.Date).Distinct().ToList();

            var json = System.Text.Json.JsonSerializer.Serialize(dates, new JsonSerializerOptions() { ReferenceHandler = ReferenceHandler.IgnoreCycles });
            return Ok(json);
        }

        [HttpGet("GetAllSchedule")]
        public IActionResult GetAllScreenings()
        {
            var schedule = _context.Screenings
            .Include(x => x.Movie)
            .Include(x => x.Auditorium)
            .Select(x => new ScreeningDto()
            {
                Id = x.Id,
                ScreeningStamp = x.ScreeningStamp,
                MovieId = x.MovieId,
                AuditoriumId = x.AuditoriumId,
                Cost = x.Cost,
                AuditoriumDto = new AuditoriumDto()
                {
                    Id = x.AuditoriumId,
                    SeatsNo = x.Auditorium.SeatsNo,
                    Title = x.Auditorium.Title,
                },
                MovieDto = new MovieDto(x.Movie)
            })
            .OrderBy(x => x.Id).ToList();

            var json = System.Text.Json.JsonSerializer.Serialize(schedule, new JsonSerializerOptions() { ReferenceHandler = ReferenceHandler.IgnoreCycles });
            return Ok(json);
        }

        [HttpPost("AddScreening")]
        public async Task<IActionResult> AddNewScreening([FromForm] ScheduleRequest request)
        {
            var newScreening = new Screening()
            {
                MovieId = request.MovieId,
                AuditoriumId = request.AuditoriumId,
                ScreeningStamp = request.ScreeningStamp,
                Cost = request.Cost,
            };

            _context.Screenings.Add(newScreening);
            await _context.SaveChangesAsync();

            return Ok("Добавлен новый сеанс!");
        }

        public class ScheduleRequest
        {
            public int MovieId { get; set; }
            public int AuditoriumId { get; set; }
            public DateTime ScreeningStamp { get; set; }
            public int Cost { get; set; }
        }

        [HttpDelete("DeleteScreening/{id}")]
        public IActionResult DeleteScreening(int id)
        {
            var screening = _context.Screenings.Where(x => x.Id == id).FirstOrDefault();
            _context.Screenings.Remove(screening);
            _context.SaveChanges();
            return Ok("Deleted");
        }
    }
}
