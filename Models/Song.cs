using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace Musikdatenbank.Models
{
    public class Song
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [MaxLength(200, ErrorMessage = "Title can't exceed 200 characters.")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "Artist is required.")]
        [MaxLength(200, ErrorMessage = "Artist name can't exceed 200 characters.")]
        public string? Artist { get; set; }

        [MaxLength(200, ErrorMessage = "Album name can't exceed 200 characters.")]
        public string? Album { get; set; }

        [MaxLength(100, ErrorMessage = "Genre can't exceed 100 characters.")]
        public string? Genre { get; set; }

        [Display(Name = "Release Date")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Release date is required.")]
        public DateTime ReleaseDate { get; set; }

        [Url(ErrorMessage = "File path must be a valid URL.")]
        public string? FilePath { get; set; }

        [NotMapped]
        public IFormFile? Mp3File { get; set; }
    }
}
