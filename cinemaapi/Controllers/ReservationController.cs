﻿using cinemaapi.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace cinemaapi.Controllers
{
    public class ReservationController : Controller
    {
        private readonly CinemaContext _context;
        public ReservationController(CinemaContext context)
        {
            _context = context;
        }


        [HttpPost("ReserveSeats")]
        public IActionResult ReserveSeats([FromForm] ReserveSeatsRequest request)
        {
            var seatsToReserve = _context.Seats
                .Where(s => request.SeatIds.Contains(s.Id))
                .ToList();

            if (seatsToReserve.Count != request.SeatIds.Count)
            {
                return BadRequest("Одно или несколько мест не существует.");
            }

            var occupiedSeats = _context.SeatReserveds
                .Where(sr => request.SeatIds.Contains(sr.SeatId) && sr.Reservation.ScreeningId == request.ScreeningId)
                .Select(sr => sr.SeatId)
                .ToList();

            if (occupiedSeats.Count > 0)
            {
                return BadRequest($"Места с идентификаторами {string.Join(", ", occupiedSeats)} уже заняты.");
            }

            var newReserv = new Reservation
            {
                ScreeningId = request.ScreeningId,
                UserId = request.UserId,
                PurchaseDate = request.PurchaseDate,
            };

            _context.Reservations.Add(newReserv);
            _context.SaveChanges();

            var newReservations = seatsToReserve
                .Select(seat => new SeatReserved
                {
                    SeatId = seat.Id,
                    ReservationId = newReserv.Id
                })
                .ToList();

            _context.SeatReserveds.AddRange(newReservations);
            _context.SaveChanges();

            return Ok(new { message = "Места успешно забронированы.", id = newReserv.Id });
        }

        public class ReserveSeatsRequest
        {
            public List<int> SeatIds { get; set; }
            public int ScreeningId { get; set; }
            public int UserId { get; set; }
            public DateTime PurchaseDate { get; set; }
        }

        [HttpPost("RefundTickets")]
        public IActionResult RefundTickets([FromForm] RefundTicketsRequest request)
        {
            var reservation = _context.Reservations
                .Include(r => r.SeatReserveds)
                .FirstOrDefault(r => r.Id == request.ReservationId);

            if (reservation == null)
            {
                return BadRequest("Бронирование не найдено.");
            }

            // Check if the screening has already started
            var screening = _context.Screenings.Find(reservation.ScreeningId);
            if (screening.ScreeningStamp <= DateTime.Now)
            {
                return BadRequest("Невозможно вернуть билеты на уже начавшийся сеанс.");
            }

            // Remove the seat reservations
            _context.SeatReserveds.RemoveRange(reservation.SeatReserveds);

            // Remove the reservation
            _context.Reservations.Remove(reservation);

            _context.SaveChanges();

            return Ok("Билеты успешно возвращены.");
        }

        public class RefundTicketsRequest
        {
            public int ReservationId { get; set; }
        }

        [HttpGet("GetReservationsByScreening/{screeningId}")]
        public IActionResult GetReservationsByScreening(int screeningId)
        {
            var reservations = _context.Reservations
                .Include(r => r.User)
                .Include(r => r.SeatReserveds)
                .Where(r => r.ScreeningId == screeningId)
                .Select(r => new
                {
                    r.Id,
                    userName = r.User.Username,
                    r.PurchaseDate,
                    seatCount = r.SeatReserveds.Count,
                    totalCost = r.SeatReserveds.Count * r.Screening.Cost
                })
                .ToList();

            return Ok(reservations);
        }
    }
}
