using Oblig2VerkhalantsevaAPI.Models;

namespace Oblig2VerkhalantsevaAPI.Services;

public interface IBookService
{
    Task<List<Book>> GetAll();
    
    Book? Get(int id);
    
    Task Save(Book book);
    
    Task Delete(int id);
    
}