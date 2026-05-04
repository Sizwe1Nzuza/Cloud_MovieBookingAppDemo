using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieBookingAppDemo.Data;
using MovieBookingAppDemo.Models;
using MovieBookingAppDemo.Services;


namespace MovieBookingAppDemo.Controllers
{
    public class MoviesController : Controller
    {
        private readonly DataContext _context;
        private readonly BlobService _blobService;

        //Constructor
        public MoviesController(DataContext context, BlobService blobService)
        {
            _context = context;
            _blobService = blobService;
        }

        // GET: Movies
        public async Task<IActionResult> Index()
        {
            return View(await _context.Movies.ToListAsync());
        }

        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .FirstOrDefaultAsync(m => m.MovieId == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // GET: Movies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Movies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]


        //since we are using images, change from Bind to IFormFile so that a file can work
        public async Task<IActionResult> Create(Movie movie, IFormFile imageFile) //async -> when user uploads stuff, page will reload with all the new stuff
        {
            //checks if the user selelected a file
            if (imageFile == null || imageFile.Length == 0)
               {
                ModelState.AddModelError("", "Please select an Image file.");
               }

            //ONle continues if there's no validation error
            if (ModelState.IsValid)
            {
                try //prevents application from crashing
                {
                    //Uploads file (image) to local blob storage
                    movie.ImageUrl = await _blobService.UploadFileAsync(imageFile, "movie-images"); //'movie-images' name will appear in azure blob storage, will be sent as an image

                    //checks if the file upload failed
                    if (string.IsNullOrEmpty(movie.ImageUrl))
                    {
                        ModelState.AddModelError("", "Image Upload Failed. No URL was returned.");
                        return View(movie); //shows error in the same page
                    }

                    //saves entry to the database (including the image URL)
                    _context.Add(movie);  //as it goes to db, will be saved as a string      
                    await _context.SaveChangesAsync();

                    //redirect to index page after successful
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // if something goes wrong when uploading -> show error message
                    ModelState.AddModelError("", "Upload failed:" + ex.Message);
                }
            }
            return View(movie);
            //If validation fails, return the same view with entered data and errors
       
        }

        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MovieId,Title,Genre,ImageUrl")] Movie movie)
        {
            if (id != movie.MovieId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.MovieId))
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
            return View(movie);
        }

        // GET: Movies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .FirstOrDefaultAsync(m => m.MovieId == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //searches the DB
            var movie = await _context.Movies.Include(m => m.TicketBookings)
                .FirstOrDefaultAsync(m => m.MovieId == id);

            if (movie == null)
            {
                return NotFound();
            }

            //this prevents deletion if associated with a booking:
            if (movie.TicketBookings.Any())
            {
                TempData["Error"] = "Cannot delete Movie with active bookings.";
                return View(movie);
            }
            //If the movie does not have a booking, it will be removed from the webpage as well as the DB
            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }

        private bool MovieExists(int id)
        {
            return _context.Movies.Any(e => e.MovieId == id);
        }

    }
}
