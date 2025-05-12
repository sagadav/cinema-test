using cinemaapi.Model;

namespace cinemaapi.DTO
{
    public class ScreeningDto
    {
        public int Id { get; set; }

        public int MovieId { get; set; }

        public int AuditoriumId { get; set; }

        public DateTime ScreeningStamp { get; set; }

        public int Cost { get; set; }

        public virtual AuditoriumDto AuditoriumDto { get; set; } = null!;

        public virtual MovieDto MovieDto { get; set; } = null!;
    }
}
