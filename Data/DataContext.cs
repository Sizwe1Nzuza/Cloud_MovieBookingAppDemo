using Microsoft.EntityFrameworkCore;
using MovieBookingAppDemo.Models;

namespace MovieBookingAppDemo.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Cinema> Cinemas { get; set; }
        public DbSet<TicketBooking> TicketBookings { get; set; }
    }
}