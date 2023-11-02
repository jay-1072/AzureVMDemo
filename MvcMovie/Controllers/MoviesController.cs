using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcMovie.Data;
using MvcMovie.Models;
using System.Data;

namespace MvcMovie.Controllers;

public class MoviesController : Controller
{
    private readonly MvcMovieContext _context;

    public MoviesController(MvcMovieContext context)
    {
            _context = context;
    }
    public async Task<IActionResult> Index(string searchString)
    { 
        if(_context.Movie == null)
        {
            return Problem("Entity set 'MvcMovieContext.Movie'  is null.");
        }

        var movies = _context.Movie.Where(m => String.IsNullOrEmpty(searchString) || m.Title!.Contains(searchString));

        return View(await movies.ToListAsync());
    }
    public async Task<IActionResult> Index1(string searchString)
    {
        if (_context.Movie == null)
        {
            return Problem("Entity set 'MvcMovieContext.Movie'  is null.");
        }

        var movies = _context.Movie.Where(m => String.IsNullOrEmpty(searchString) || m.Title!.Contains(searchString));

        return PartialView("_Index", await movies.ToListAsync());
    }

    public async Task<IActionResult> Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Movie model)
    {
        try
        {
            if(ModelState.IsValid)
            {
                _context.Movie.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
        }
        catch (DataException)
        {
            return Problem("Oops something went wrong! Try again");
        }
        return View(model);
    }


    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var movie = await _context.Movie.FirstOrDefaultAsync(Movie => Movie.Id == id);
    
        if(movie == null)
        {
            return NotFound();
        }
        return View(movie);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if(id == null)
        {
            return NotFound();
        }

        var movie = await _context.Movie.FindAsync(id);
        if(movie == null)
        {
            return NotFound();
        }

        return View(movie);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id, Title, ReleaseDate, Genre, Price")] Movie model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if(!MovieExists(model.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction("Index");
        }
        return View(model);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if(id == null)
        {
            return NotFound();
        }

        var movie = await _context.Movie.FindAsync(id);
        if(movie == null)
        {
            return NotFound();
        }
        return View(movie);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, Movie model)
    {
        if(id != model.Id)
        {
            return NotFound();
        }

        try
        {
            _context.Movie.Remove(model); 
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        catch(Exception)
        {
            return View("Error");
        }
    }

    public bool MovieExists(int id)
    {
        var movie = _context.Movie.FirstOrDefault(m => m.Id == id);
        if (movie != null)
        {
            return true;
        }

        return false;
    }
}
