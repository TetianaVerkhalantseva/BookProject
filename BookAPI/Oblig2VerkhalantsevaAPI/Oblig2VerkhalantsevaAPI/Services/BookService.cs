using Oblig2VerkhalantsevaAPI.Models;

namespace Oblig2VerkhalantsevaAPI.Services;

public class BookService : IBookService
{
    
    private static List<Book> Books { get; }
    private static int _nextId = 4;

    static BookService()
    {
        Books = new List<Book>
        {
            new Book { Id = 1, Title = "The Great Gatsby", Description = "A classic novel by F. Scott Fitzgerald", Year = 1925, AuthorId = 0, CategoryId = 0, PublisherId = 0, LanguageId = 0 },
            new Book { Id = 2, Title = "To Kill a Mockingbird", Description = "A novel by Harper Lee about racial injustice", Year = 1960, AuthorId = 0, CategoryId = 0, PublisherId = 0, LanguageId = 0 },
            new Book { Id = 3, Title = "1984", Description = "A dystopian novel by George Orwell", Year = 1949, AuthorId = 0, CategoryId = 0, PublisherId = 0, LanguageId = 0 }
        };
    }
    
    
    public async Task<List<Book>> GetAll() => await Task.FromResult(Books);
    
    public Book? Get(int id) => Books.FirstOrDefault(b => b.Id == id);

    public Task Save(Book book)
    {
        var existingBook = Books.FirstOrDefault(b => b.Id == book.Id);
        if (existingBook != null)
        {
            existingBook.Title = book.Title;
            existingBook.Description = book.Description;
            existingBook.Year = book.Year;
        }
        else
        {
            book.Id = _nextId++;
            Books.Add(book);
        }
        
        return Task.CompletedTask;
    }

    public async Task Delete(int id)
    {
        var book = Get(id);
        if (book is null)
            return;
        
        Books.Remove(book);
        await Task.Yield();

    }
    
}