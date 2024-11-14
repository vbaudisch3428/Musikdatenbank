using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MvcSong.Data;
using System;
using System.Linq;

namespace Musikdatenbank.Models;

public static class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
{
    using (var context = new MvcSongContext(
        serviceProvider.GetRequiredService<
            DbContextOptions<MvcSongContext>>()))
    {
        // Überprüfe, ob es bereits Songs gibt
        if (context.Song.Any())
        {
            return;   // DB ist bereits initialisiert
        }

        context.Song.AddRange(
            new Song
            {
                Title = "Thunderous Night",
                Artist = "V!NCE",
                Album = "TGM Hits",
                Genre = "Hardstyle",
                ReleaseDate = DateTime.Parse("2024-11-14"),
                FilePath = "/music/ThunderousNight.mp3"  // Setze hier den Pfad der Datei
            },
            new Song
            {
                Title = "Code and Chaos",
                Artist = "V!NCE",
                Album = "TGM Hits",
                Genre = "Intense Electric Rock",
                ReleaseDate = DateTime.Parse("2024-11-14"),
                FilePath = "/music/CodeAndChaos.mp3"  // Setze hier den Pfad der Datei
            }
        );
        context.SaveChanges();
    }
}

}