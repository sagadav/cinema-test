using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using System.Text.Json;
using cinemaapi.Model;
using System;
using System.Globalization;

namespace cinemaapi.Controllers
{
    public class AuditoriumController : Controller
    {
        private readonly CinemaContext _context;
        public AuditoriumController(CinemaContext context)
        {
            _context = context;
        }


        [HttpGet("GetHalls")]
        public IActionResult GetHalls()
        {
            var halls = _context.Auditorium.Select(x => new { x.Id, x.Title }).ToList();

            var json = System.Text.Json.JsonSerializer.Serialize(halls, new JsonSerializerOptions() { ReferenceHandler = ReferenceHandler.IgnoreCycles });
            return Ok(json);
        }
    }
}
