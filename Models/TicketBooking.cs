using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieBookingAppDemo.Models
{
    public class TicketBooking
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TicketBookingId { get; set; }

        // Foreign Keys
        public int MovieId { get; set; }
        public Movie? Movie { get; set; }

        public int CinemaId { get; set; }
        public Cinema? Cinema { get; set; }

        public DateTime ShowDate { get; set; }

    }
}
