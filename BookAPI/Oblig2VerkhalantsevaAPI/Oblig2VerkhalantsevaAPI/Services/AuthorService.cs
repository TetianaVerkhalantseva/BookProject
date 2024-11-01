using Microsoft.EntityFrameworkCore;
using Oblig2VerkhalantsevaAPI.Data;
using Oblig2VerkhalantsevaAPI.Models;
using Oblig2VerkhalantsevaAPI.Models.Entities;

namespace Oblig2VerkhalantsevaAPI.Services;

public class AuthorService : IAuthorService
{
    private readonly ApplicationDbContext _db;

    public AuthorService(ApplicationDbContext db)
    {
        _db = db;
    }
    
    public async Task<IEnumerable<AuthorDto>> GetAllAuthors()
    {
        return await _db.Author
            .Select(a => new AuthorDto
            {
                Id = a.Id,
                FirstName = a.FirstName,
                LastName = a.LastName
            })
            .ToListAsync();
    }
    
    public async Task<AuthorDto?> GetAuthorDto(int id)
    {
        var author = await _db.Author.FindAsync(id);
        if (author == null) return null;
        
        return new AuthorDto
        {
            Id = author.Id,
            FirstName = author.FirstName,
            LastName = author.LastName
        };
    }
    
    public async Task<AuthorDto?> GetAuthorByName(string firstName, string lastName)
    {
        var author = await _db.Author
            .FirstOrDefaultAsync(a => (a.FirstName != null && a.FirstName.ToLower() == firstName.ToLower()) &&
                                      a.LastName.ToLower() == lastName.ToLower());
        
        if (author == null) return null;

        return new AuthorDto
        {
            Id = author.Id,
            FirstName = author.FirstName,
            LastName = author.LastName
        };
    }

    // Save or update an author
    public async Task Save(Author author)
    {
        var existingAuthor = await _db.Author.FindAsync(author.Id);
        if (existingAuthor != null)
        {
            _db.Entry(existingAuthor).State = EntityState.Detached;
        }
        
        _db.Author.Update(author);
        await _db.SaveChangesAsync();
    }
    
    public async Task Delete(int id)
    {
        var author = await _db.Author.FindAsync(id);
        if (author == null) throw new InvalidOperationException($"No author with id {id} found.");
        
        _db.Author.Remove(author);
        await _db.SaveChangesAsync();
    }
}
