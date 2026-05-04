namespace MovieBookingAppDemo.Models
{
    public class Movie
    {
        public int MovieId { get; set; }
        public string? Title { get; set; }
        public string? Genre { get; set; }

        public string? ImageUrl { get; set; }

        // Navigation property
        public List<TicketBooking> TicketBookings { get; set; } = new List<TicketBooking>();


    }
}
