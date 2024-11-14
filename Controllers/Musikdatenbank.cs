using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Musikdatenbank.Models;
using MvcSong.Data;
using Musikdatenbank.Controllers;

namespace Musikdatenbank.Controllers
{
    public class Musikdatenbank : Controller
    {
        private readonly MvcSongContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;


        public Musikdatenbank(MvcSongContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;

        }
        public async Task<IActionResult> Index(string movieGenre, string searchString, string sortOrder)
        {
            var songs = from m in _context.Song
                        select m;

            if (!string.IsNullOrEmpty(searchString))
            {
                songs = songs.Where(s => s.Title.Contains(searchString) ||
                                          s.Artist.Contains(searchString) ||
                                          s.Genre.Contains(searchString) ||
                                          s.Album.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(movieGenre))
            {
                songs = songs.Where(x => x.Genre == movieGenre);
            }

            // Sortierung hinzufügen
            ViewData["TitleSortParm"] = string.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            switch (sortOrder)
            {
                case "title_desc":
                    songs = songs.OrderByDescending(s => s.Title);
                    break;
                default:
                    songs = songs.OrderBy(s => s.Title);
                    break;
            }

            var songGenreVM = new SongGenreViewModel
            {
                Songs = await songs.ToListAsync(),
                SearchString = searchString
            };

            return View(songGenreVM);
        }



        // GET: Musikdatenbank/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var song = await _context.Song
                .FirstOrDefaultAsync(m => m.Id == id);
            if (song == null)
            {
                return NotFound();
            }

            return View(song);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Artist,Album,Genre,ReleaseDate,Mp3File")] Song song)
        {
            if (ModelState.IsValid)
            {
                // Datei hochladen und Song speichern
                if (song.Mp3File != null)
                {
                    var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "music", song.Mp3File.FileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await song.Mp3File.CopyToAsync(stream);
                    }

                    song.FilePath = $"/music/{song.Mp3File.FileName}";
                }

                _context.Add(song);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index)); // nach dem Erstellen zur Index-Seite zurück
            }
            return View(song);
        }

        // GET: Musikdatenbank/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var song = await _context.Song.FindAsync(id);
            if (song == null)
            {
                return NotFound();
            }
            return View(song);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Artist,Album,Genre,ReleaseDate,Mp3File")] Song song)
        {
            if (id != song.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (song.Mp3File != null)
                    {
                        // Speichern der MP3-Datei
                        var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "music", song.Mp3File.FileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await song.Mp3File.CopyToAsync(stream);
                        }

                        // Setze den FilePath auf den gespeicherten Pfad
                        song.FilePath = $"/music/{song.Mp3File.FileName}";
                    }

                    _context.Update(song);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SongExists(song.Id))
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
            return View(song);
        }


        // GET: Musikdatenbank/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var song = await _context.Song
                .FirstOrDefaultAsync(m => m.Id == id);
            if (song == null)
            {
                return NotFound();
            }

            return View(song);
        }

        // POST: Musikdatenbank/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var song = await _context.Song.FindAsync(id);
            if (song != null)
            {
                _context.Song.Remove(song);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SongExists(int id)
        {
            return _context.Song.Any(e => e.Id == id);
        }
    }
}
