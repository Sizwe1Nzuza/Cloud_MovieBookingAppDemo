using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieBookingAppDemo.Data;
using MovieBookingAppDemo.Models;

namespace MovieBookingAppDemo.Controllers
{
    public class TicketBookingsController : Controller
    {
        private readonly DataContext _context;

        public TicketBookingsController(DataContext context)
        {
            _context = context;
        }

        // GET: TicketBookings
        public async Task<IActionResult> Index()
        {
            var dataContext = _context.TicketBookings.Include(t => t.Cinema).Include(t => t.Movie);
            return View(await dataContext.ToListAsync());
        }

        // GET: TicketBookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketBooking = await _context.TicketBookings
                .Include(t => t.Cinema)
                .Include(t => t.Movie)
                .FirstOrDefaultAsync(m => m.TicketBookingId == id);
            if (ticketBooking == null)
            {
                return NotFound();
            }

            return View(ticketBooking);
        }

        // GET: TicketBookings/Create
        public IActionResult Create()
        {
            ViewData["CinemaId"] = new SelectList(_context.Cinemas, "CinemaId", "CinemaId");
            ViewData["MovieId"] = new SelectList(_context.Movies, "MovieId", "Title");
            return View();
        }

        // POST: TicketBookings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TicketBookingId,MovieId,CinemaId,ShowDate")] TicketBooking ticketBooking)
        {
            if (ModelState.IsValid)
            {
                // Check for double booking before it gets saved 
                var existingBooking = _context.TicketBookings
                    .Any(b => b.CinemaId == ticketBooking.CinemaId
                           && b.ShowDate == ticketBooking.ShowDate);

                if (existingBooking)
                {
                    TempData["Error"] = "This booking is not available for the selected date/time. Please select another cinema or choose a different time.";
                    //another way to write the error message: 
                    // model state eror: ModelState.AddModelError("", "This cinema is already booked for the selected date/time.");

                    //reloads the dropdowns
                    ViewData["CinemaId"] = new SelectList(_context.Cinemas, "CinemaId", "CinemaId", ticketBooking.CinemaId);
                    ViewData["MovieId"] = new SelectList(_context.Movies, "MovieId", "Title", ticketBooking.MovieId);

                    return View(ticketBooking);
                }

                _context.Add(ticketBooking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(ticketBooking);
        }




            //Validation to ensure MovieId exists:
            //if (!_context.Movies.Any(m => m.MovieId == ticketBooking.MovieId))
            //{
            //    ModelState.AddModelError("MovieId", "Movie ID does not exist");
            //}

            ////Validation to ensure CinemaId exists:
            //if (!_context.Cinemas.Any(m => m.CinemaId == ticketBooking.CinemaId))
            //{
            //    ModelState.AddModelError("CinemaId", "Cinema ID does not exist");
            //}


            //if (ModelState.IsValid)
            //{
            //    _context.Add(ticketBooking);
            //    await _context.SaveChangesAsync();
            //    return RedirectToAction(nameof(Index));
            //}
            ////ViewData["CinemaId"] = new SelectList(_context.Cinemas, "CinemaId", "CinemaId", ticketBooking.CinemaId);
            //ViewData["MovieId"] = new SelectList(_context.Movies, "MovieId", "MovieId"); //I removed this after MovieId: , ticketBooking.MovieId
            //return View(); //I removed this from inside the view():  ticketBooking
        

        // GET: TicketBookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketBooking = await _context.TicketBookings.FindAsync(id);
            if (ticketBooking == null)
            {
                return NotFound();
            }
            ViewData["CinemaId"] = new SelectList(_context.Cinemas, "CinemaId", "CinemaId", ticketBooking.CinemaId);
            ViewData["MovieId"] = new SelectList(_context.Movies, "MovieId", "MovieId", ticketBooking.MovieId);
            return View(ticketBooking);
        }

        // POST: TicketBookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TicketBookingId,MovieId,CinemaId,ShowDate")] TicketBooking ticketBooking)
        {
            if (id != ticketBooking.TicketBookingId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ticketBooking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketBookingExists(ticketBooking.TicketBookingId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CinemaId"] = new SelectList(_context.Cinemas, "CinemaId", "CinemaId", ticketBooking.CinemaId);
            ViewData["MovieId"] = new SelectList(_context.Movies, "MovieId", "MovieId", ticketBooking.MovieId);
            return View(ticketBooking);
        }

        // GET: TicketBookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketBooking = await _context.TicketBookings
                .Include(t => t.Cinema)
                .Include(t => t.Movie)
                .FirstOrDefaultAsync(m => m.TicketBookingId == id);
            if (ticketBooking == null)
            {
                return NotFound();
            }

            return View(ticketBooking);
        }

        // POST: TicketBookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticketBooking = await _context.TicketBookings.FindAsync(id);
            if (ticketBooking != null)
            {
                _context.TicketBookings.Remove(ticketBooking);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TicketBookingExists(int id)
        {
            return _context.TicketBookings.Any(e => e.TicketBookingId == id);
        }
    }
}
