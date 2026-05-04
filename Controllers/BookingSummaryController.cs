using Microsoft.AspNetCore.Mvc;
using MovieBookingAppDemo.Data;
using Microsoft.EntityFrameworkCore;
using MovieBookingAppDemo.Models; 

namespace MovieBookingAppDemo.Controllers
{
    public class BookingSummaryController : Controller
    {
        //controller: takes info form model and sends updated info to View
        //Since viewModel isn't a database entity, no need for migrations
        



        //connects to database      -->     do this to retrieve info to display to user as ViewModel
        private readonly DataContext _context;

        //uses dependency injection to automatically make the connection
        public BookingSummaryController(DataContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(string search) //will take the user's search (word) and filters it throughout the below code
        {
            //Load information from database
            var bookings = _context.TicketBookings //required info from DB retrieved
                    .Include(b => b.Movie)
                    .Include(b => b.Cinema)
                    .AsQueryable(); //makes the data filterable (we are able to filter through it/ apply filter to it)
                    // keeps the ting on the side and will display later

            //search filter (Booking ID or Movie Title)


            // user can only search/filter by TicketBookingId or Movie.Title
            if (!string.IsNullOrEmpty(search)) //only runs search if the user types something i search bar
                {
                bookings = bookings.Where(b =>
                    b.TicketBookingId.ToString().Contains(search) ||
                    b.Movie.Title.Contains(search));
                // bookings refers to these 3-ish lines of code
                }


            // Below code: searches through all of the bookings, takes booking you need and
            //Map to ViewModel (combining data from 3 tables into 1)
            var result = await bookings.Select(b => new BookingSummary //for each booking, create a ViewModel
            {



                BookingId = b.TicketBookingId, //info from these 2 models equals the same thing (BookingId)  --> 2 Models: TicketBooking and BookingSummary
                BookingDate = b.ShowDate,

                MovieTitle = b.Movie.Title,
                MovieGenre = b.Movie.Genre,

                CinemaName = b.Cinema.Name,
                CinemaLocation = b.Cinema.Location
            }).ToListAsync();   //execute the query and store the result in list
                                //ToLIst will display all of the searches

            Console.WriteLine($"Search term: {search}");
            return View(result);

        }
    }
}
