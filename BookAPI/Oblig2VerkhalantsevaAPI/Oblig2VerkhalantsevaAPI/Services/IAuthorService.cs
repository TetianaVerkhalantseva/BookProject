using Oblig2VerkhalantsevaAPI.Models;
using Oblig2VerkhalantsevaAPI.Models.Entities;

namespace Oblig2VerkhalantsevaAPI.Services;

public interface IAuthorService
{
    Task<IEnumerable<AuthorDto>> GetAllAuthors();
    Task<AuthorDto?> GetAuthorDto(int id);
    Task<AuthorDto?> GetAuthorByName(string firstName, string lastName);
    Task Save(Author author);
    Task Delete(int id);
}
