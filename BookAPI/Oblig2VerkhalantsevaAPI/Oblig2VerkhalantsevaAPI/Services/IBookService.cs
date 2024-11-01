using Oblig2VerkhalantsevaAPI.Models;

namespace Oblig2VerkhalantsevaAPI.Services;

public interface IBookService
{
    Task<IEnumerable<BookDto>> GetAllBooks();

    Task<BookDto?> GetBook(int id);

    Task<BookDto?> GetBookByTitle(string title);

    Task Save(Book book);

    Task Delete(int id);

}