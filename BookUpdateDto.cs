using System.ComponentModel.DataAnnotations;

public class BookUpdateDto
{
    [Required]
    public int BookId { get; set; } 

    [Required(ErrorMessage = "Title is required.")]
    public string Title { get; set; }

    [Required(ErrorMessage = "Author is required.")]
    public string Author { get; set; }

    public string Genre { get; set; }

    [Range(1900, 2100, ErrorMessage = "Please enter a valid year between 1900 and 2100.")]
    public string? PublicationYear { get; set; } 

    [Required(ErrorMessage = "ISBN is required.")]
    [RegularExpression(@"\d{5}$", ErrorMessage = "Please enter a valid ISBN in the format 97823")]
    public string ISBN { get; set; }

    public string Description { get; set; }
}
