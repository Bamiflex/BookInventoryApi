
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Claims; 

[Route("api/[controller]")]
[ApiController]
[Authorize] 
public class BooksController : ControllerBase
{
    private readonly BookContext _context;

    public BooksController(BookContext context)
    {
        _context = context;
    }

    private int GetUserIdFromToken()
    {
        var userIdClaim = User.FindFirst("UserId")?.Value;

        if (int.TryParse(userIdClaim, out int userId))
        {
            return userId;
        }
        else
        {
            throw new FormatException("Invalid UserId format in token.");
        }
    }


    // GET: api/Books
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
    {
        int userId = GetUserIdFromToken(); 

        // Return only books that belong to the authenticated user
        return await _context.Books
                             .Where(b => b.UserId == userId)
                             .ToListAsync();
    }

    // POST: api/Books
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Book>> AddBook(BookCreateDto bookDto)
    {
        int userId = GetUserIdFromToken(); 

        // Check if a book with the same ISBN already exists for this user
        if (await _context.Books.AnyAsync(b => b.ISBN == bookDto.ISBN && b.UserId == userId))
        {
            return BadRequest(new { message =  "A book with the same ISBN already exists." });
        }

        // Create a new book entity
        var newBook = new Book
        {
            Title = bookDto.Title,
            Author = bookDto.Author,
            Genre = bookDto.Genre,
            PublicationYear = bookDto.PublicationYear,
            ISBN = bookDto.ISBN,
            Description = bookDto.Description,
            UserId = userId 
        };

        _context.Books.Add(newBook);
        await _context.SaveChangesAsync();

        // Return the newly created book
        return CreatedAtAction(nameof(GetBook), new { id = newBook.BookId }, newBook);
    }


    // GET: api/Books/{id}
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<Book>> GetBook(int id)
    {
        int userId = GetUserIdFromToken(); 

        var book = await _context.Books
                                 .Where(b => b.UserId == userId && b.BookId == id)
                                 .FirstOrDefaultAsync();

        if (book == null)
        {
            return NotFound("The book was not found or you do not have permission to view it.");
        }

        return book;
    }

    // PUT: api/Books/{id}
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateBook(int id, BookUpdateDto bookDto)
    {
        int userId = GetUserIdFromToken(); 

        if (id != bookDto.BookId)
        {
            return BadRequest("The book ID does not match the URL parameter.");
        }

        var bookToUpdate = await _context.Books
                                         .Where(b => b.UserId == userId && b.BookId == id)
                                         .FirstOrDefaultAsync();

        if (bookToUpdate == null)
        {
            return NotFound("The book with the specified ID was not found or you do not have permission to edit it.");
        }

        if (await _context.Books.AnyAsync(b => b.ISBN == bookDto.ISBN && b.BookId != id))
        {
            return BadRequest("A book with the same ISBN already exists.");
        }

        // Update book details
        bookToUpdate.Title = bookDto.Title;
        bookToUpdate.Author = bookDto.Author;
        bookToUpdate.Genre = bookDto.Genre;
        bookToUpdate.PublicationYear = bookDto.PublicationYear;
        bookToUpdate.ISBN = bookDto.ISBN;
        bookToUpdate.Description = bookDto.Description;

        _context.Entry(bookToUpdate).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!BookExists(id))
            {
                return NotFound("The book with the specified ID was not found.");
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // DELETE: api/Books/{id}
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteBook(int id)
    {
        int userId = GetUserIdFromToken(); 

        var book = await _context.Books
                                 .Where(b => b.UserId == userId && b.BookId == id)
                                 .FirstOrDefaultAsync();

        if (book == null)
        {
            return NotFound("The book was not found or you do not have permission to delete it.");
        }

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // GET: api/Books/search?query=somequery
    [HttpGet("search")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<Book>>> SearchBooks([FromQuery] string query)
    {
        int userId = GetUserIdFromToken(); 

        var results = await _context.Books
            .Where(b => b.UserId == userId && (
                        b.Title.ToLower().Contains(query.ToLower()) ||
                        b.Author.ToLower().Contains(query.ToLower()) ||
                        b.Genre.ToLower().Contains(query.ToLower()) ||
                        b.Description.ToLower().Contains(query.ToLower())))
            .ToListAsync();

        if (results.Count == 0)
        {
            return NotFound("No books match the search criteria.");
        }

        return results;
    }

    private bool BookExists(int id)
    {
        return _context.Books.Any(e => e.BookId == id);
    }
}


