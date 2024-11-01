namespace Oblig2VerkhalantsevaAPI.Models;

public class BookDto //Data Transfer Object
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public int Year { get; set; }
    public AuthorDto? Author { get; set; } 
    public CategoryDto? Category { get; set; } 
    public PublisherDto? Publisher { get; set; }
    public LanguageDto? Language { get; set; }
}