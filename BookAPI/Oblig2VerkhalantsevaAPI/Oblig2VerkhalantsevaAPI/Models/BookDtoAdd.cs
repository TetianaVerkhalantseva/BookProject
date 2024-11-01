namespace Oblig2VerkhalantsevaAPI.Models;

public class BookDtoAdd
{
    public int Id { get; set; }
    public string Title { get; set; } = null!; // Title should never be null, but is temporarily initialized as null for now
    public string? Description { get; set; }
    public int Year { get; set; }
    public int AuthorId { get; set; } 
    public int CategoryId { get; set; }
    public int PublisherId { get; set; } 
    public int LanguageId { get; set; }
}