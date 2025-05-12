using cinemaapi.Model;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace cinemaapi.Controllers
{
    public class UserController : Controller
    {
        private readonly CinemaContext _context;
        public UserController(CinemaContext context)
        {
            _context = context;
        }

        [HttpPost("Login")]
        public IActionResult GetLogin([FromBody] LoginRequest request)
        {
            PasswordHasher<User> passwordHasher = new PasswordHasher<User>();

            var user = _context.Users.ToList().Find(x => x.Email == request.Email);
            if (user == null)
            {
                return NotFound("Пользователь не найден!");
            }

            var newPass = passwordHasher.VerifyHashedPassword(user, user.Password, request.Password);

            if (newPass == 0)
            {
                return NotFound("Неверная почта или пароль!");
            }

            var json = System.Text.Json.JsonSerializer.Serialize(user, new JsonSerializerOptions() { ReferenceHandler = ReferenceHandler.IgnoreCycles });
            return Ok(json);
        }

        public class LoginRequest
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        [HttpPost("Registration")]
        public IActionResult GetRegistration([FromBody] RegistrationRequest request)
        {
            var isUserExist = _context.Users.ToList().Find(x => x.Email == request.Email);
            if (isUserExist != null)
            {
                return NotFound("Данная почта уже занята!");
            }

            var user = new User()
            {
                Email = request.Email,
                Password = request.Password,
                Username = request.Username,
                Role = "User"
            };

            PasswordHasher<User> passwordHasher = new PasswordHasher<User>();
            user.Password = passwordHasher.HashPassword(user, request.Password);

            _context.Users.Add(user);
            _context.SaveChanges();
            var json = System.Text.Json.JsonSerializer.Serialize(user, new JsonSerializerOptions() { ReferenceHandler = ReferenceHandler.IgnoreCycles });
            return CreatedAtAction(nameof(GetRegistration), json);
        }

        public class RegistrationRequest
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public string Email { get; set; }
        }

        [HttpPatch("ChangeUserDetails/{id}")]
        public IActionResult UpdateUserData([FromBody] UpdateRequest request)
        {
            var user = _context.Users.ToList().Find(x => x.Id == request.Id);
            if (user == null)
            {
                return NotFound("Пользователь не найден!");
            }

            user.Username = request.Username;
            PasswordHasher<User> passwordHasher = new PasswordHasher<User>();
            user.Password = passwordHasher.HashPassword(user, request.Password);

            _context.SaveChanges();

            var json = System.Text.Json.JsonSerializer.Serialize(user, new JsonSerializerOptions() { ReferenceHandler = ReferenceHandler.IgnoreCycles });

            return Ok(user);
        }

        public class UpdateRequest
        {
            public int Id { get; set; }
            public string Password { get; set; }
            public string Username { get; set; }
        }

        //[HttpGet("History/{id}")]
        //public IActionResult GetUserHistory(int id)
        //{
        //    var tickets = _context.Reservations
        //    .Where(x => x.UserId == id)
        //    .Include(x => x.Screening)
        //        .ThenInclude(x => x.Movie)
        //    .GroupBy(x => new
        //    {
        //        ScreeningId = x.Screening.Id,
        //        MovieTitle = x.Screening.Movie.Title,
        //        MoviePoster = x.Screening.Movie.Poster,
        //        Cost = x.Screening.Cost,
        //        ScreeningDate = x.Screening.ScreeningStamp
        //    })
        //    .Select(group => new
        //    {
        //        group.Key.ScreeningId,
        //        group.Key.MovieTitle,
        //        group.Key.MoviePoster,
        //        group.Key.Cost,
        //        group.Key.ScreeningDate,
        //        group.First().PurchaseDate,
        //        TicketCount = group.Count()
        //    })
        //    .ToList();

        //    return Ok(tickets);
        //}
        [HttpGet("History/{id}")]
        public IActionResult GetUserHistory(int id)
        {
            var tickets = _context.Reservations
                .Where(reservation => reservation.UserId == id)
                .Include(reservation => reservation.Screening)
                    .ThenInclude(screening => screening.Movie)
                .GroupBy(reservation => new
                {
                    ScreeningId = reservation.Screening.Id,
                    MovieTitle = reservation.Screening.Movie.Title,
                    MoviePoster = reservation.Screening.Movie.Poster,
                    Cost = reservation.Screening.Cost,
                    ScreeningDate = reservation.Screening.ScreeningStamp
                })
                .Select(group => new
                {
                    group.Key.ScreeningId,
                    group.Key.MovieTitle,
                    group.Key.MoviePoster,
                    group.Key.Cost,
                    group.Key.ScreeningDate,
                    TicketCount = group.Sum(r => r.SeatReserveds.Count),
                    PurchaseDate = group.FirstOrDefault().PurchaseDate
                })
                .ToList();

            return Ok(tickets);
        }

        [HttpGet("Users")]
        public IActionResult GetAllUsers()
        {
            var users = _context.Users
                .Select(u => new
                {
                    u.Id,
                    u.Username,
                    u.Email
                })
                .ToList();

            return Ok(users);
        }
    }
}
