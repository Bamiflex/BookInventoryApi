
using System.ComponentModel.DataAnnotations;

public class Book
{
    public int BookId { get; set; }

    [Required(ErrorMessage = "Title is required.")]
    public string Title { get; set; }

    [Required(ErrorMessage = "Author is required.")]
    public string Author { get; set; }

    [Required(ErrorMessage = "Genre is required.")]
    public string? Genre { get; set; }

    [Required(ErrorMessage = "PublicationYear is required.")]
    public string PublicationYear { get; set; }

    [Required(ErrorMessage = "ISBN is required.")]
    [StringLength(5, MinimumLength = 5, ErrorMessage = "ISBN should be between 5 characters long.")]
    public string ISBN { get; set; }

    [Required(ErrorMessage = "Description is required.")]
    public string? Description { get; set; }

    public int UserId { get; set; }
}
