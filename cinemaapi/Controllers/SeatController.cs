using cinemaapi.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace cinemaapi.Controllers
{
    public class SeatController : Controller
    {
        private readonly CinemaContext _context;
        public SeatController(CinemaContext context)
        {
            _context = context;
        }

        public class SeatInfo
        {
            public int SeatId { get; set; }
            public int SeatNum { get; set; }
            public int SeatRow { get; set; }
            public bool IsReserved { get; set; }
        }

        public List<SeatInfo> GetAvailableSeats(int screeningId)
        {
            var screening = _context.Screenings
                .Include(s => s.Auditorium)
                .FirstOrDefault(s => s.Id == screeningId);

            if (screening == null)
            {
                return null;
            }

            var allSeats = _context.Seats
                .Where(seat => seat.AuditoriumId == screening.AuditoriumId)
            .ToList();

            var occupiedSeatIds = _context.SeatReserveds
                .Include(x => x.Reservation)
                .ThenInclude(x => x.Screening)
                .Where(sr => sr.Reservation.ScreeningId == screeningId)
                .Select(sr => sr.SeatId)
                .ToList();

            var availableSeats = allSeats
                .Select(seat => new SeatInfo
                {
                    SeatId = seat.Id,
                    SeatNum = seat.SeatNum,
                    SeatRow = seat.SeatRow,
                    IsReserved = occupiedSeatIds.Contains(seat.Id) ? true : false,
                })
                .ToList();

            return availableSeats;
        }

        [HttpGet("GetAvailableSeats/{screeningId}")]
        public IActionResult GetAvailableSeatsAtScreening(int screeningId)
        {
            var availableSeats = GetAvailableSeats(screeningId);

            if (availableSeats == null || !availableSeats.Any())
            {
                return NotFound("Свободных мест для данного сеанса не найдено.");
            }

            return Ok(availableSeats);
        }
    }
}
