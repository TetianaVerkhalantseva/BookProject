using Microsoft.EntityFrameworkCore;
using Oblig2VerkhalantsevaAPI.Data;
using Oblig2VerkhalantsevaAPI.Models;

namespace Oblig2VerkhalantsevaAPI.Services;

public class BookService : IBookService
{
    private readonly ApplicationDbContext _db;

    public BookService(ApplicationDbContext db)
    {
        _db = db;
    }
    
    // Get all books with related data (Author, Category, Publisher, Language) using DTO
    public async Task<IEnumerable<BookDto>> GetAllBooks()
    {
        try
        {
            var books = await _db.Book
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Include(b => b.Publisher)
                .Include(b => b.Language)
                .ToListAsync();

            var returnBooks = books.Select(b => new BookDto
            {
                Id = b.Id,
                Title = b.Title,
                Description = b.Description,
                Year = b.Year,
                Author = new AuthorDto
                {
                    Id = b.Author.Id,
                    FirstName = b.Author.FirstName,
                    LastName = b.Author.LastName
                } ,
                Category = new CategoryDto
                {
                    Id = b.Category.Id,
                    Name = b.Category.Name
                },
                Publisher = new PublisherDto
                {
                    Id = b.Publisher.Id,
                    Name = b.Publisher.Name
                } ,
                Language = new LanguageDto
                {
                    Id = b.Language.Id,
                    Name = b.Language.Name
                }
            }).ToList();

            return returnBooks;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);

            return new List<BookDto>();
        }
    }
    
    // Get a single book by Id using DTO
    public async Task<BookDto?> GetBook(int id)
    {
        try
        {
            var book = await _db.Book
                .Where(b => b.Id == id)
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Include(b => b.Publisher)
                .Include(b => b.Language)
                .FirstOrDefaultAsync();

            if (book == null) return null;
            var bookDto = new BookDto
            {   
                Id = book.Id,
                Title = book.Title,
                Description = book.Description,
                Year = book.Year,
                Author = new AuthorDto
                {
                    Id = book.Author.Id,
                    FirstName = book.Author.FirstName,
                    LastName = book.Author.LastName
                },
                Category = new CategoryDto
                {
                    Id = book.Category.Id,
                    Name = book.Category.Name
                },
                Publisher = new PublisherDto
                {
                    Id = book.Publisher.Id,
                    Name = book.Publisher.Name
                },
                Language = new LanguageDto
                {
                    Id = book.Language.Id,
                    Name = book.Language.Name
                }
            };

            return bookDto;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    // Get a single book by Title using DTO
    public async Task<BookDto?> GetBookByTitle(string title)
    {
        try
        {
            var book = await _db.Book
                .Where(b => b.Title.ToLower() == title.ToLower())
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Include(b => b.Publisher)
                .Include(b => b.Language)
                .FirstOrDefaultAsync();

            if (book == null) return null;

            var bookDto = new BookDto
            {
                Id = book.Id,
                Title = book.Title,
                Description = book.Description,
                Year = book.Year,
                Author = new AuthorDto
                {
                    Id = book.Author.Id,
                    FirstName = book.Author.FirstName,
                    LastName = book.Author.LastName
                },
                Category = new CategoryDto
                {
                    Id = book.Category.Id,
                    Name = book.Category.Name
                },
                Publisher = new PublisherDto
                {
                    Id = book.Publisher.Id,
                    Name = book.Publisher.Name
                },
                Language = new LanguageDto
                {
                    Id = book.Language.Id,
                    Name = book.Language.Name
                }
            };

            return bookDto;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }


    // Save or update a book
    public async Task Save(Book book)
    {
        var existingBook = await _db.Book.FindAsync(book.Id);
        if (existingBook != null)
        {
            _db.Entry(existingBook).State = EntityState.Detached;
        }

        _db.Book.Update(book);
        await _db.SaveChangesAsync();
    }
    
    // Delete a book by Id
    public async Task Delete(int id)
    {
        var book = _db.Book.FindAsync(id);

        // Remove the book from the database
        _db.Book.Remove(await book ?? throw new InvalidOperationException($"No book with id {id} found."));
        await _db.SaveChangesAsync();
    }
    
}